using Combinatorics
using JuMP
using Gurobi
using Statistics

"""
Island

# Arguments
- `id`: island id (one-indexed)
- `r`: row containing island
- `c`: column containg island
- `k`: number of bridges to be connected to the island
- `neighbours`: vector of the ids of islands to which a bridge could be build
"""
struct Island
    id::Int64
    r::Int64
    c::Int64
    k::Int64
    neighbours::Vector{Int64}
end


"""
benchmark(pattern)

Benchmark the Hashiwokakero solver for instance data set

# Arguments
- `pattern`: regex for all files
"""
function benchmark(pattern::String, lazy = true)
    # Get all files matching the pattern
    dir_path = dirname(pattern)
    base_name = basename(pattern)
    files = filter(f -> startswith(f, base_name), readdir(dir_path))
    full_paths = [joinpath(dir_path, f) for f in files]
    
    # Solve each instance and collect the times
    times = Float64[]
    for file in full_paths
        try
            solve_time = lazy ? solveLazy(file) : solve(file)
            push!(times, solve_time)
        catch e
            println("Error processing file $file: $e")
        end
    end

    avg_time = isempty(times) ? 0.0 : mean(times)
    
    return avg_time, times
end


"""
solve(file)

Solves the Hashiwokakero puzzle using a polynomial encoding and reports the runtime in seconds.

# Arguments
- `file`: name of the file to read
"""
function solve(file::String, external::Bool = false)
    islands, intersections = readFile(file)
    n = length(islands)

    e = 0
    edgeMap = Dict{Int64, Tuple{Int64, Int64}}()  # (edge id) -> (id of two islands connecting edge)
    revEdgeMap = Dict{Tuple{Int64, Int64}, Int64}()  # (id of two islands connecting edge) -> (edge id)
    for i in 1:n
        for j in islands[i].neighbours
            if i < j  # count edge only once
                e += 1
                edgeMap[e] = (i, j)
                revEdgeMap[(i, j)] = e
            end
        end
    end
    m = length(edgeMap)

    model = Model(Gurobi.Optimizer)
    @variable(model, x[1:m, 1:2], Bin)  # x[e, l] iff at least l bridges are built on edge e
    @variable(model, y[1:m, 1:m], Bin)  # y[e, l] iff edge e can be reached from source edge in <= l - 1 steps

    # by defintion x[e, 2] implies x[e, 1]
    @constraint(model, [e=1:m], x[e, 2] <= x[e, 1])

    # 1) Correct bridge count
    for i in 1:n
        bridge_sum = AffExpr(0)  # Initialize empty expression
        for j in islands[i].neighbours
            if i < j  # edge exists in forward direction
                e = revEdgeMap[(i,j)]
                bridge_sum += x[e,1] + x[e,2]
            else  # edge exists in reverse direction
                e = revEdgeMap[(j,i)]
                bridge_sum += x[e,1] + x[e,2]
            end
        end
        @constraint(model, bridge_sum == islands[i].k)
    end

    # 2) No intersections
    for (a,b,c,d) in intersections
        # Get the correct edge ids, ensuring we look up in the right order
        e1 = a < b ? revEdgeMap[(a,b)] : revEdgeMap[(b,a)]
        e2 = c < d ? revEdgeMap[(c,d)] : revEdgeMap[(d,c)]
        @constraint(model, x[e1,1] + x[e2,1] <= 1)
    end

    # 3) Connected bridges
    @constraint(model, sum(y[:, 1]) == 1)  # exactly one source edge
    @constraint(model, [e=1:m, l=1:m-1], y[e,l] <= y[e,l+1])  # if edge e can be reached from source in <= l - 1 steps, then also in <= l steps
    @constraint(model, [e=1:m], x[e,1] == y[e,m])  # every used edge must be reachable from source in <= m - 1 step

    # if edge can be reached from source in <= l steps, then it or one of its neighbours must be reached in <= l - 1 steps
    for e in 1:m
        for l in 1:m-1
            (i1, i2) = edgeMap[e]
            # Find all edges that share an endpoint with edge e
            neighboring_edges = AffExpr(0)
            for j in islands[i1].neighbours
                edge_id = i1 < j ? revEdgeMap[(i1,j)] : revEdgeMap[(j,i1)]
                neighboring_edges += y[edge_id, l]
            end
            for j in islands[i2].neighbours
                edge_id = i2 < j ? revEdgeMap[(i2,j)] : revEdgeMap[(j,i2)]
                neighboring_edges += y[edge_id, l]
            end
            @constraint(model, y[e,l+1] <= neighboring_edges + y[e, l])
        end
    end

    if external
        path = splitext(basename(file))[1]
        JuMP.write_to_file(model, "$path.mps")
        return
    end

    optimize!(model)

    status = termination_status(model)
    if status == MOI.OPTIMAL
        println("Solution found:")
        prettyPrint(islands, value.(x), edgeMap)
    else
        println("Problem is unsatisfiable.")
    end

    return round(solve_time(model); digits = 3)
end


"""
solveLazy(file)

Solves the Hashiwokakero puzzle using lazy constraints and reports the runtime in seconds.

# Arguments
- `file`: name of the file to read
"""
function solveLazy(file::String)
    islands, intersections = readFile(file)
    n = length(islands)

    e = 0
    edgeMap = Dict{Int64, Tuple{Int64, Int64}}()  # (edge id) -> (id of two islands connecting edge)
    revEdgeMap = Dict{Tuple{Int64, Int64}, Int64}()  # (id of two islands connecting edge) -> (edge id)
    for i in 1:n
        for j in islands[i].neighbours
            if i < j  # count edge only once
                e += 1
                edgeMap[e] = (i, j)
                revEdgeMap[(i, j)] = e
            end
        end
    end
    m = length(edgeMap)

    model = Model(Gurobi.Optimizer)
    @variable(model, x[1:m, 1:2], Bin)  # x[e, l] iff at least l bridges are built on edge e

    # by defintion x[e, 2] implies x[e, 1]
    @constraint(model, [e=1:m], x[e, 2] <= x[e, 1])

    # 1) Correct bridge count
    for i in 1:n
        bridge_sum = AffExpr(0)  # Initialize empty expression
        for j in islands[i].neighbours
            if i < j  # edge exists in forward direction
                e = revEdgeMap[(i,j)]
                bridge_sum += x[e,1] + x[e,2]
            else  # edge exists in reverse direction
                e = revEdgeMap[(j,i)]
                bridge_sum += x[e,1] + x[e,2]
            end
        end
        @constraint(model, bridge_sum == islands[i].k)
    end

    # 2) No intersections
    for (a,b,c,d) in intersections
        # Get the correct edge ids, ensuring we look up in the right order
        e1 = a < b ? revEdgeMap[(a,b)] : revEdgeMap[(b,a)]
        e2 = c < d ? revEdgeMap[(c,d)] : revEdgeMap[(d,c)]
        @constraint(model, x[e1,1] + x[e2,1] <= 1)
    end

    # 3) lazy ccnnectivity constraints
    function lazy_callback(cb_data)
        status = callback_node_status(cb_data, model)
        if status != MOI.CALLBACK_NODE_STATUS_INTEGER
            return  # Only run at integer solutions
        end

        cuts = bfsAll(islands, callback_value.(cb_data, model[:x]), edgeMap, revEdgeMap)

        if length(cuts) == 1 && isempty(cuts[1])
            # all islands connected
            return
        end

        for cut in cuts
            con = @build_constraint(sum(model[:x][e, 1] for e in cut) >= 1)
            MOI.submit(model, MOI.LazyConstraint(cb_data), con)
        end

        return
    end

    set_attribute(model, MOI.LazyConstraintCallback(), lazy_callback)
    optimize!(model)

    @assert(isConnected(islands, value.(x), edgeMap))

    return round(solve_time(model); digits = 3)
end


"""
bfsAll(islands, x, edgeMap, revEdgeMap)

Check if all islands are connected.

# Arguments
- `islands`: vector of island structs
- `x`: solution of ILP model
- `edgeMap`: map edge id to endpoint ids
- `revEdgeMap`: map endpoint ids to edge id
"""
function isConnected(islands::Vector{Island}, x::Array{Float64,2}, edgeMap::Dict{Int64, Tuple{Int64, Int64}})
    n = length(islands)
    m = length(edgeMap)

    # Create adjacency list representation based on solution x
    adj = Dict(i => Int[] for i in 1:n)
    for e in 1:m
        if x[e,1] > 0.5
            (i, j) = edgeMap[e]
            push!(adj[i], j)
            push!(adj[j], i)
        end
    end

    start = 1
    visited = Set{Int}([])
    queue = [start]
    push!(visited, start)

    while !isempty(queue)
        current = popfirst!(queue)
        for neighbor in adj[current]
            if !(neighbor in visited)
                push!(visited, neighbor)
                push!(queue, neighbor)
            end
        end
    end

    return length(visited) == n
end


"""
bfsAll(islands, x, edgeMap, revEdgeMap)

Return the respective edge cuts for all connected components.

# Arguments
- `islands`: vector of island structs
- `x`: solution of ILP model
- `edgeMap`: map edge id to endpoint ids
- `revEdgeMap`: map endpoint ids to edge id
"""
function bfsAll(islands::Vector{Island}, x::Array{Float64,2}, edgeMap::Dict{Int64, Tuple{Int64, Int64}}, revEdgeMap::Dict{Tuple{Int64, Int64}, Int64})
    n = length(islands)
    m = length(edgeMap)

    # Create adjacency list representation based on solution x
    adj = Dict(i => Int[] for i in 1:n)
    for e in 1:m
        if x[e,1] > 0.5
        (i, j) = edgeMap[e]
        push!(adj[i], j)
        push!(adj[j], i)
        end
    end

    # Keep track of all islands we've seen
    all_visited = Set{Int}()
    component_cuts = Vector{Int}[]

    while length(all_visited) < n
        # Start new component from first unvisited island
        start = 0
        for i in 1 : n
            if !(i in all_visited)
                start = i
            end
        end

        # BFS for this component
        component = Set{Int}([])
        queue = [start]
        push!(component, start)

        while !isempty(queue)
            current = popfirst!(queue)
            for neighbor in adj[current]
                if !(neighbor in component)
                    push!(component, neighbor)
                    push!(queue, neighbor)
                end
            end
        end

        # Find cut edges for this component
        cut_edges = Int[]
        for i in component
            for j in islands[i].neighbours
                if !(j in component)
                    edge_id = i < j ? revEdgeMap[(i,j)] : revEdgeMap[(j,i)]
                    push!(cut_edges, edge_id)
                end
            end
        end

        push!(component_cuts, cut_edges)
        union!(all_visited, component)
    end

    return component_cuts
end


"""
prettyPrint(islands, x)

# Arguments
- `islands`: vector of island structs
- `x`: solution of ILP model
"""
function prettyPrint(islands::Vector{Island}, x::Array{Float64,2}, edgeMap::Dict{Int64, Tuple{Int64, Int64}})
    # Find grid dimensions
    rows = maximum(island.r for island in islands)
    cols = maximum(island.c for island in islands)
    
    # Create empty grid with double size plus 1 for spacing
    grid = fill(" ", 2*rows + 1, 2*cols + 1)
    
    # Place islands at their scaled positions
    for island in islands
        grid[2*island.r - 1, 2*island.c - 1] = string(island.k)
    end
    
    # Place bridges using the edge map
    n = length(islands)
    for e in 1:size(x,1)
        if x[e,1] > 0.5  # At least one bridge exists
            num_bridges = x[e,1] + x[e,2] > 1.5 ? 2 : 1
            (i,j) = edgeMap[e]
            island_a = islands[i]
            island_b = islands[j]
            
            bridge_char = if island_a.r == island_b.r  # Horizontal bridge
                num_bridges == 2 ? "=" : "-"
            else  # Vertical bridge
                num_bridges == 2 ? "‖" : "|"
            end
            
            # Place bridge
            if island_a.r == island_b.r  # Horizontal bridge
                start_c = min(2*island_a.c - 1, 2*island_b.c - 1)
                end_c = max(2*island_a.c - 1, 2*island_b.c - 1)
                for c in (start_c+1):(end_c-1)
                    grid[2*island_a.r - 1, c] = bridge_char
                end
            else  # Vertical bridge
                start_r = min(2*island_a.r - 1, 2*island_b.r - 1)
                end_r = max(2*island_a.r - 1, 2*island_b.r - 1)
                for r in (start_r+1):(end_r-1)
                    grid[r, 2*island_a.c - 1] = bridge_char
                end
            end
        end
    end
    
    # Print the grid
    for r in 1:(2*rows + 1)
        println(join(grid[r,:]))
    end
end


"""
readFile(file)

# Arguments
- `file`: name of the file to read
"""
function readFile(file::String)
    lines = readlines(file)
    nrows, ncols, nislands = parse.(Int, split(lines[1]))
    
    grid = zeros(Int, nrows, ncols)
    islandMap = Dict{Tuple{Int,Int}, Int}()  # (r,c) => id
    islands = Island[]
    intersections = Vector{Tuple{Int64, Int64, Int64, Int64}}()
    
    # Create islands and build position map
    currId = 1
    for r in 1:nrows
        grid[r,:] = parse.(Int, split(lines[r+1]))

        for c in 1:ncols
            if grid[r,c] > 0
                push!(islands, Island(currId, r, c, grid[r,c], Vector{Int64}()))
                islandMap[(r,c)] = currId
                currId += 1
            end
        end
    end
    
    # Find neighbors
    for island in islands
        for (dr, dc) in [(0, -1), (0, 1), (1, 0), (-1, 0)]
            r, c = island.r + dr, island.c + dc
            while 1 <= r <= nrows && 1 <= c <= ncols
                if haskey(islandMap, (r,c))
                    push!(island.neighbours, islandMap[(r,c)])
                    break
                end
                r, c = r + dr, c + dc
            end
        end
    end

    # Find intersections as non-island grid cells with four island neighbours
    for r in 1:nrows
        for c in 1:ncols
            if !haskey(islandMap, (r,c))
                neighIds = Int64[]
                # Look in all four directions
                for (dr, dc) in [(0, -1), (0, 1), (1, 0), (-1, 0)]
                    rr, cc = r + dr, c + dc
                    while 1 <= rr <= nrows && 1 <= cc <= ncols
                        if haskey(islandMap, (rr, cc))
                            push!(neighIds, islandMap[(rr, cc)])
                            break
                        end
                        rr, cc = rr + dr, cc + dc
                    end
                end
                
                # If we found islands in all directions, it's an intersection
                if length(neighIds) == 4
                    push!(intersections, (neighIds[1], neighIds[2], neighIds[3], neighIds[4]))
                end
            end
        end
    end

    return islands, intersections
end

# benchmark("dataset/100/")
# benchmark("dataset/200/")
# benchmark("dataset/300/")
# benchmark("dataset/400/")

# (c) Mia Muessig
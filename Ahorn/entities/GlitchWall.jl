module FurryHelperGlitchWall

using ..Ahorn, Maple

@mapdef Entity "FurryHelper/GlitchWall" GlitchWall(x::Integer, y::Integer, width::Integer=16, height::Integer=16, tiletype::String="m", BPM::Integer=120, TimeDelays::String="1,1,2")

const placements = Ahorn.PlacementDict(
    "GlitchWall (FurryHelper)" => Ahorn.EntityPlacement(
        GlitchWall,
        "rectangle",
        Dict{String, Any}(),
        function(entity)
            entity.data["nodes"] = [(Int(entity.data["x"]) + Int(entity.data["width"]) + 8, Int(entity.data["y"]))]
            Ahorn.tileEntityFinalizer(entity)
        end,
    ),
)

Ahorn.nodeLimits(entity::GlitchWall) = 1,1

Ahorn.editingOptions(entity::GlitchWall) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions()
)

Ahorn.minimumSize(entity::GlitchWall) = 8, 8
Ahorn.resizable(entity::GlitchWall) = true, true

function Ahorn.selection(entity::GlitchWall)
    x, y = Ahorn.position(entity)
    nx, ny = Int.(entity.data["nodes"][1])

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    return [Ahorn.Rectangle(x, y, width, height), Ahorn.Rectangle(nx, ny, width, height)]
end

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::GlitchWall, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity, blendIn=true)

function Ahorn.renderSelectedAbs(ctx::Ahorn.Cairo.CairoContext, entity::GlitchWall, room::Maple.Room)
    x, y = Ahorn.position(entity)
    nodes = get(entity.data, "nodes", ())

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))
    
    if !isempty(nodes)
        nx, ny = Int.(nodes[1])
        cox, coy = floor(Int, width / 2), floor(Int, height / 2)

        material = get(entity.data, "tiletype", "3")[1] 

        fakeTiles = Ahorn.createFakeTiles(room, nx, ny, width, height, material, blendIn=true)
        Ahorn.drawFakeTiles(ctx, room, fakeTiles, room.objTiles, true, nx, ny, clipEdges=true)
        Ahorn.drawArrow(ctx, x + cox, y + coy, nx + cox, ny + coy, Ahorn.colors.selection_selected_fc, headLength=6)
    end
end

end

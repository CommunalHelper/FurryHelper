module FurryHelperMomentumCameraOffset

using ..Ahorn, Maple

@mapdef Trigger "FurryHelper/momentumCameraOffsetTrigger" MomentumCameraOffsetTrigger(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    offsetXFrom::Number=0.0, offsetXTo::Number=0.0, offsetYFrom::Number=0.0, offsetYTo::Number=0.0,
    momentumFrom::Number=0.0, momentumTo::Number=0.0,
    momentumMode::String="VerticalMomentum", onlyOnce::Bool=false, xOnly::Bool=false, yOnly::Bool=false)

const placements = Ahorn.PlacementDict(
    "Momentum Camera Offset (FurryHelper)" => Ahorn.EntityPlacement(
        MomentumCameraOffsetTrigger,
        "rectangle",
    ),
)

function Ahorn.editingOptions(trigger::MomentumCameraOffsetTrigger)
    return Dict{String, Any}(
        "momentumMode" => String[
            "HorizontalMomentum",
            "VerticalMomentum"
        ]
    )
end

end

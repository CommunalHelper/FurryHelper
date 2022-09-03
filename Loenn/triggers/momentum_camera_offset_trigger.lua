local momentumCameraOffsetTrigger = {}

momentumCameraOffsetTrigger.name = "FurryHelper/momentumCameraOffsetTrigger"

momentumCameraOffsetTrigger.placements = {
    name = "momentum_camera",
    data = {
        offsetXFrom = 0.0,
        offsetXTo = 0.0,
        offsetYFrom = 0.0,
        offsetYTo = 0.0,
        momentumFrom = 0.0,
        momentumTo = 0.0,
        momentumMode = "VerticalMomentum",
        onlyOnce = false,
        xOnly = false,
        yOnly = false
    }
}

momentumCameraOffsetTrigger.fieldInformation = {
    momentumMode = {
        options = {
            "HorizontalMomentum",
            "VerticalMomentum"
        },
        editable = false
    }
}

momentumCameraOffsetTrigger.fieldOrder = {
    "x", "y", "width", "height", 
    "offsetXFrom", "offsetXTo", "offsetYFrom", "offsetYTo", 
    "momentumFrom", "momentumTo", "momentumMode", 
    "onlyOne", "xOnly", "yOnly"
}

return momentumCameraOffsetTrigger
local fakeTilesHelper = require("helpers.fake_tiles")
local utils = require("utils")

local glitchWall = {}

glitchWall.name = "FurryHelper/GlitchWall"
glitchWall.depth = -10501
glitchWall.nodeLimits = {1, 1}
glitchWall.nodeLineRenderType = "line"

glitchWall.placements = {
    name = "glitch_wall",
    data = {
        tiletype = "m",
        BPM = 120,
        TimeDelays = "1,1,2",
        width = 16,
        height = 16
    }
}

glitchWall.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

glitchWall.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", true)

function glitchWall.selection(room, entity)
    local nodes = entity.nodes or {}
    local x, y = entity.x or 0, entity.y or 0
    local nodeX, nodeY = nodes[1].x or x, nodes[1].y or y
    local width, height = entity.width or 16, entity.height or 16

    return utils.rectangle(x, y, width, height), {utils.rectangle(nodeX, nodeY, width, height)}
end

return glitchWall
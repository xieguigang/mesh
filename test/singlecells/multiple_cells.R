require(mesh);
require(GCModeller);
require(mzkit);
require(graphics);
require(graphics2D);

imports "mzweb" from "mzkit";

setwd(@dir);

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 100, 
    mzdiff = 0.005);
let raster = as.raster(readImage(`../../docs\104_A10_1_blue_red_green.jpg`)); 

bitmap(file = "./raster_multiple1.png", size = [1920, 1920]);
rasterHeatmap(raster);
dev.off();

samples.raster(mesh, raster);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE);

write.mzPack(pack, file = `singlecells-multiple.mzPack`, version = 2);
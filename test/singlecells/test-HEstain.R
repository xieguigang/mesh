require(mesh);
require(GCModeller);
require(mzkit);
require(graphics);
require(graphics2D);

imports "mzweb" from "mzkit";
imports "clustering" from "MLkit";
imports "dataset" from "MLkit";

setwd(@dir);

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 125, 
    mzdiff = 0.005);
let raster = as.raster(readImage(`../../docs\HE-1024x676.jpg`)); 

# let labels = raster_vec(raster);
# let gmm = clustering::gmm(labels, components = 6);
# let gauss = gmm.predict_proba(gmm);

# print(labels);

# labels = gmm.predict(gmm);
# # labels = q_factors(labels, levels = 3);

# print(labels);

bitmap(file = "./raster-HE.png", size = [1920, 1080]);
rasterHeatmap(raster);
dev.off();

samples.raster(mesh, raster);
# samples.raster(mesh, raster, label = labels);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE, q= 0.3);

write.mzPack(pack, file = `HEstain-singlecells.mzPack`, version = 2);
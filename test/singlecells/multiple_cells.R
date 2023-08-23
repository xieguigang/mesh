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
    feature.size = 100, 
    mzdiff = 0.005);
let raster = as.raster(readImage(`../../docs\104_A10_1_blue_red_green.jpg`)); 

# let labels = raster_vec(raster);
# # let gmm = clustering::gmm(labels, components = 6);
# # let gauss = gmm.predict_proba(gmm);

# print(labels);

# labels = q_factors(labels, levels = 9);

# print(labels);

# labels = gmm.predict(gmm);

# print(gauss, max.print = 6);
# print(labels);
# print("unique labels:");
# print(unique(labels));

# write.csv(gauss, file = "./gauss.csv", row.names = TRUE);

bitmap(file = "./raster_multiple1.png", size = [1920, 1920]);
rasterHeatmap(raster);
dev.off();

# samples.raster(mesh, raster, label = labels);
samples.raster(mesh, raster,linear.kernel = TRUE);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE, q = 0);

write.mzPack(pack, file = `singlecells-multiple.mzPack`, version = 2);
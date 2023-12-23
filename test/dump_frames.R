imports "dataset" from "MLkit";

require(JSON);
require(CNN);

setwd("F:/");

let dataset = read.sample_set(file = "./spectrum.dat");
dataset = sort_samples(dataset, order_id = 5, desc = TRUE);

let imgs = list.files("./AtlasSlices", pattern = "*.jpg");
let global = [0, 1];

print(imgs);

for(path in imgs) {
    let raster = as.raster(readImage(path)); 
    let v = raster_vec(ratser);
    let minmax = range(v);

    print(minmax);

    stop();
}
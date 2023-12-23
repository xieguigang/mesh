imports "dataset" from "MLkit";

require(JSON);
require(CNN);

setwd(@dir);

let dataset = read.sample_set(file = "./spectrum.dat");
dataset = sort_samples(dataset, order_id = 5, desc = TRUE);

let imgs = list.files("./AtlasSlices", pattern = "*.jpg");

print(imgs);

for(path in imgs) {
    
}
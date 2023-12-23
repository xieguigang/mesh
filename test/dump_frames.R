imports "dataset" from "MLkit";

require(JSON);
require(CNN);
require(graphics);

setwd("F:/");

let dataset = read.sample_set(file = "./spectrum.dat");
let metadata = "metadata.json"
|> readText()
|> JSON::json_decode()
;

dataset = sort_samples(dataset, order_id = 5, desc = TRUE);

let imgs = list.files("./AtlasSlices", pattern = "*.jpg");
let global = [0, 1];
let decoder = CNN::cnn("./spectrum.cnn");

print(imgs);
str(metadata);

for(path in imgs) {
    let raster = as.data.frame(as.raster(readImage(path))); 
    raster = raster[raster$scale > 0, ];
    let i = as.integer(length(dataset) * raster$scale);
    let seeds = dataset[i];
    let x = raster$x;
    let y = raster$y;
    let total = get_feature(seeds, );
    let spectrum_data = decoder(seeds, class_labels = as.character(metadata$ions),
                                 is_generative = TRUE);

    print(raster, max.print = 6);
    print(i);
    str(spectrum_data);

    stop();
}


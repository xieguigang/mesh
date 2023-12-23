imports "dataset" from "MLkit";

require(JSON);
require(CNN);
require(graphics);
require(mzkit);

imports ["SingleCells", "MSI"] from "mzkit";

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
let res = [ 528, 320 ];

print(imgs);
str(metadata);
imgs =["F:\\AtlasSlices\\071.jpg"];
CNN::n_threads(32);

for(path in imgs) {
    let raster = as.data.frame(as.raster(readImage(path))); 
    raster = raster[raster$scale > 0, ];
    let i = as.integer(length(dataset) * raster$scale);
    let seeds = dataset[i];
    let x = raster$x;
    let y = raster$y;
    let total = get_feature(seeds, 5);
    let spectrum_data = decoder(seeds, class_labels = as.character(metadata$ions),
                                 is_generative = TRUE);

    rownames(spectrum_data) = `${x},${y}`;

    print(raster, max.print = 6);
    print(i);
    print(total);
    str(spectrum_data);

    let msi_data = t(spectrum_data);
     msi_data = MSI::scale(msi_data, total, bpc = TRUE);
        msi_data = MSI::levels.convolution(msi_data, 
            win_size = 6, clusters = 9);
    msi_data = t(msi_data);
    let save_export = `${dirname(path)}/${basename(path)}.imzML`;

    let rawdata = MSI::pack_matrix(file = msi_data, res, source.tag = `AtlasSlices_${basename(path)}`);

    write.imzML(rawdata, file = save_export, res = 30);

   # stop();
}


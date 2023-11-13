#' A helper function for create kegg compound ion features
#' 
#' @param dbfile a file path to the HDS package database file, example as:
#'   ``/GCModeller/src/repository/graphquery/kegg/tools/cache.db``.
#' @param category the kegg pathway category name for get the kegg
#'   compound list. This pathway category name must be end with a directory
#'   seperator symbol ``/``.
#' 
const pull_kegg = function(dbfile, category = "/Metabolism/Energy metabolism/") {
    let kegg_db = HDS::openStream(dbfile, readonly = TRUE);
    let files = as.data.frame(HDS::files(
        kegg_db, 
        dir = category, recursive = TRUE)
    );

    # print(as.data.frame(files));
    files = files[files$type == "dir", ];
    files = files[basename(files$path) == "compounds", ];

    # get all compounds
    files = files$path;
    files = files 
    |> lapply(function(dir) {
        as.data.frame(HDS::files(kegg_db, dir = `${dir}/`, recursive = TRUE));
    }) 
    |> lapply(df -> df$path)
    |> unlist()
    ;

    files = data.frame(files, name = basename(files));
    files = files 
    |> groupBy("name") 
    |> sapply(function(d) {
        const list = (d$files);

        if (length(list) == 1) {
            list;
        } else {
            .Internal::first(list);
        }
    });

    print(files);

    let compounds = lapply(files, function(path) {
        try({
            loadXml(HDS::getText(kegg_db, fileName = path), typeof = "kegg_compound");
        });
    });

    names(compounds) = basename(files);

    return(compounds);
}
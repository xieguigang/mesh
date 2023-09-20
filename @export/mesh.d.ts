// export R# package module type define for javascript/typescript language
//
//    imports "mesh" from "Mesh";
//
// ref=mesh.Rscript@Mesh, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * MSdata expression matrix simulator for metabolomics analysis pipeline development and test.
 * 
*/
declare namespace mesh {
   module as {
      /**
       * Cast the data expression matrix as the mzkit mzpack object
       * 
       * 
        * @param expr1 -
        * @param mesh 
        * + default value Is ``null``.
        * @param q -
        * 
        * + default value Is ``0.7``.
        * @param rt_range -
        * 
        * + default value Is ``[1,840]``.
        * @param spatial Current expression matrix is a spatial matrix?
        * 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function mzPack(expr1: object, mesh?: object, q?: number, rt_range?: any, spatial?: boolean, env?: object): object;
   }
   /**
    * Generate the metabolomics expression matrix object
    * 
    * 
     * @param mesh -
     * @param mzpack 
     * + default value Is ``false``.
     * @param q 
     * + default value Is ``0.7``.
     * @param spatial 
     * + default value Is ``false``.
     * @return this function returns a GCModeller expression matrix object or 
     *  MZKit mzpack data object based on the parameter option of 
     *  **`mzpack`**.
   */
   function expr1(mesh: object, mzpack?: boolean, q?: number, spatial?: boolean): object|object;
   /**
    * Create a mesh argument for run metabolomics expression matrix simulation
    * 
    * 
     * @param mass_range -
     * 
     * + default value Is ``[50,1200]``.
     * @param feature_size -
     * 
     * + default value Is ``10000``.
     * @param mzdiff 
     * + default value Is ``0.005``.
     * @param intensity_max 
     * + default value Is ``100000000``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function mesh(mass_range?: any, feature_size?: object, mzdiff?: number, intensity_max?: number, env?: object): object;
   /**
    * Set metabolite features
    * 
    * 
     * @param mesh -
     * @param metabolites A collection of the metabolite annotation data model that contains 
     *  the basic annotation metadata: 
     *  
     *  1. id, 
     *  2. name, 
     *  3. exact mass, 
     *  4. and formula data
     *  
     *  this parameter value could be the annotation abstract model: @``T:BioNovoGene.BioDeep.Chemoinformatics.MetaboliteAnnotation``,
     *  or the kegg compound model @``T:SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject.Compound`` from the GCModeller package.
     * @param adducts -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function metabolites(mesh: object, metabolites: any, adducts: any, env?: object): object;
   module samples {
      /**
       * Create a spatial sample via the given raster matrix
       * 
       * 
        * @param mesh -
        * @param raster -
        * @param label 
        * + default value Is ``null``.
        * @param kernel_cutoff 
        * + default value Is ``0.0001``.
        * @param linear_kernel 
        * + default value Is ``false``.
        * @param TrIQ 
        * + default value Is ``0.85``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function raster(mesh: object, raster: object, label?: any, kernel_cutoff?: number, linear_kernel?: boolean, TrIQ?: number, env?: object): any;
      /**
       * Set spatial id
       * 
       * 
        * @param mesh -
        * @param x -
        * @param y -
        * @param z z axis of the spatial spot
        * 
        * + default value Is ``null``.
        * @param kernel 
        * + default value Is ``null``.
        * @param group 
        * + default value Is ``null``.
        * @param template 
        * + default value Is ``'[raster-%y.raw][Scan_%d][%x,%y] FTMS + p NSI Full ms [%min-%max]'``.
        * @param linear_kernel 
        * + default value Is ``false``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function spatial(mesh: object, x: any, y: any, z?: any, kernel?: any, group?: any, template?: string, linear_kernel?: boolean, env?: object): any;
   }
}

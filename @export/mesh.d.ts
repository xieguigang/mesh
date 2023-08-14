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
      function mzPack(expr1: object, q?: number, rt_range?: any, spatial?: boolean, env?: object): object;
   }
   /**
    * Generate the metabolomics expression matrix object
    * 
    * 
     * @param mesh -
     * @param mzpack 
     * + default value Is ``false``.
     * @param spatial 
     * + default value Is ``false``.
     * @return this function returns a GCModeller expression matrix object or 
     *  MZKit mzpack data object based on the parameter option of 
     *  **`mzpack`**.
   */
   function expr1(mesh: object, mzpack?: boolean, spatial?: boolean): object|object;
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
     * + default value Is ``1000000000000000``.
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
     * @param adducts -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function metabolites(mesh: object, metabolites: object, adducts: any, env?: object): object;
   module samples {
      /**
        * @param env default value Is ``null``.
      */
      function raster(mesh: object, raster: object, env?: object): any;
      /**
       * Set spatial id
       * 
       * 
        * @param mesh -
        * @param x -
        * @param y -
        * @param z 
        * + default value Is ``null``.
        * @param kernel 
        * + default value Is ``null``.
        * @param env -
        * 
        * + default value Is ``null``.
      */
      function spatial(mesh: object, x: any, y: any, z?: any, kernel?: any, env?: object): any;
   }
}

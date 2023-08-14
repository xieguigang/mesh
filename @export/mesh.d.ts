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
        * @param q default value Is ``0.7``.
        * @param rt_range default value Is ``[1,840]``.
        * @param env default value Is ``null``.
      */
      function mzPack(expr1: object, q?: number, rt_range?: any, env?: object): object;
   }
   /**
    * Generate the metabolomics expression matrix object
    * 
    * 
     * @param mesh -
     * @param mzpack 
     * + default value Is ``false``.
   */
   function expr1(mesh: object, mzpack?: boolean): object|object;
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
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function mesh(mass_range?: any, feature_size?: object, mzdiff?: number, env?: object): object;
   /**
    * Set metabolite features
    * 
    * 
     * @param mesh -
     * @param metabolites -
     * @param adducts -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function metabolites(mesh: object, metabolites: object, adducts: any, env?: object): object;
   /**
    * Set sample labels and group labels information
    * 
    * 
     * @param mesh -
     * @param sampleinfo -
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function samples(mesh: object, sampleinfo: any, env?: object): object;
}

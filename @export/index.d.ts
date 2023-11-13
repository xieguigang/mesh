// export R# source type define for javascript/typescript language
//
// package_source=mesh

declare namespace mesh {
   module _ {
      /**
      */
      function onLoad(): object;
   }
   /**
     * @param category default value Is ``/Metabolism/Energy metabolism/``.
   */
   function pull_kegg(dbfile: any, category?: any): object;
}

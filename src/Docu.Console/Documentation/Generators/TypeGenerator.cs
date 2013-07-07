using System;
using System.Collections.Generic;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class TypeGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public TypeGenerator( IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser )
            : base( commentParser )
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add( List<Namespace> namespaces, DocumentedType association )
        {
            try
            {
                if( null != namespaces && null != association )
                {
                    var ns = FindNamespace( association, namespaces );
                    if( null != ns )
                    {
                        DeclaredType doc = DeclaredType.Unresolved( (TypeIdentifier) association.Name, association.TargetType, ns );
                        if( null != doc )
                        {
                            ParseSummary( association, doc );
                            ParseRemarks( association, doc );
                            ParseValue( association, doc );
                            ParseExample( association, doc );

                            if( matchedAssociations.ContainsKey( association.Name ) ) return;

                            matchedAssociations.Add( association.Name, doc );
                            ns.AddType( doc );
                        }
                    }
                }
            }
            catch( Exception ) { }
        }

    }
}
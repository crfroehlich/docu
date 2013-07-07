using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Docu.Documentation.Comments;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal abstract class BaseGenerator
    {
        private readonly ICommentParser commentParser;

        protected BaseGenerator( ICommentParser commentParser )
        {
            this.commentParser = commentParser;
        }

        private void ParseSummary( XmlNode node, IDocumentationElement doc )
        {
            try
            {
                if( node != null )
                    doc.Summary = new Summary( commentParser.ParseNode( node ) );
            }
            catch( Exception ) { }
        }

        protected void ParseParamSummary( IDocumentationMember member, IDocumentationElement doc )
        {
            try
            {
                if( member.Xml == null ) return;

                var node = member.Xml.SelectSingleNode( "param[@name='" + doc.Name + "']" );

                ParseSummary( node, doc );
            }
            catch( Exception ) { }
        }

        protected void ParseValue( IDocumentationMember member, IDocumentationElement doc )
        {
            try
            {
                if( member.Xml == null ) return;

                var node = member.Xml.SelectSingleNode( "value" );

                if( node != null )
                    doc.Value = new Value( commentParser.ParseNode( node ) );
            }
            catch( Exception ) { }
        }

        protected void ParseSummary( IDocumentationMember member, IDocumentationElement doc )
        {
            try
            {
                if( member.Xml == null ) return;

                var node = member.Xml.SelectSingleNode( "summary" );

                ParseSummary( node, doc );
            }
            catch( Exception ) { }
        }

        protected void ParseRemarks( IDocumentationMember member, IDocumentationElement doc )
        {
            try
            {
                if( member.Xml == null ) return;

                var node = member.Xml.SelectSingleNode( "remarks" );

                if( node != null )
                    doc.Remarks = new Remarks( commentParser.ParseNode( node ) );
            }
            catch( Exception ) { }
        }

        protected void ParseExample( IDocumentationMember member, IDocumentationElement doc )
        {
            try
            {
                if( member.Xml == null ) return;

                var node = member.Xml.SelectSingleNode( "example" );

                if( node != null )
                    doc.Example = new MultilineCode( commentParser.ParseNode( node, new ParseOptions { PreserveWhitespace = true } ) );
            }
            catch( Exception ) { }
        }

        protected void ParseReturns( IDocumentationMember member, Method doc )
        {
            try
            {
                if( member.Xml == null ) return;

                var node = member.Xml.SelectSingleNode( "returns" );

                if( node != null )
                    doc.Returns = new Summary( commentParser.ParseNode( node ) );
            }
            catch( Exception ) { }
        }


        protected Namespace FindNamespace( IDocumentationMember association, List<Namespace> namespaces )
        {
            Namespace ns = null;
            try
            {
                var identifier = Identifier.FromNamespace( association.TargetType.Namespace );
                ns = namespaces.Find( x => x.IsIdentifiedBy( identifier ) );
            }
            catch( Exception ) { }
            return ns;
        }

        protected DeclaredType FindType( Namespace ns, IDocumentationMember association )
        {
            DeclaredType dt = null;
            try
            {
                var typeName = Identifier.FromType( association.TargetType );
                dt = ns.Types.FirstOrDefault( x => x.IsIdentifiedBy( typeName ) );
            }
            catch( Exception ) { }
            return dt;
        }
    }
}
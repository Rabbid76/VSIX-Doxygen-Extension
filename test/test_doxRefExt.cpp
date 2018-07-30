

#pragma region Test
/******************************************************************//**
* \brief   
* 
* [https://msdn.microsoft.com/de-de/library/system.net.mime.contenttype(v=vs.110).aspx]
* 
* @ref secShaderSSAOIntro
* \ref secShaderCodeSnippetserRangeCheck
* @ref unknown_reference
* \ref unknown_reference
* \image html parallax_cone_step.png
* @image html rim_lighting.svg
* cf \image html OffsetTopLogic_Bezug_bei_RLK.TIF
* \image html not_existing
* @image html not_existing
* \dotfile uml_example.dot
* @dotfile unknown
* \mscfile msc_example.msc
* @mscfile unknown
* \diafile test.dia
* @diafile unknown
* \brief
* @brief
* \unknown_command
* @unknown_command

\image html "Assembly UI.svg"
\image html 'Images\Block UI.svg'

\code{.glsl}
out vec4 fragColor;
main()
{
  fragColor = vec4( 1.0, 0.0, 0.0, 1.0 );
}
\endcode

[http://www.graphviz.org/pdf/dotguide.pdf]
\dot
digraph example {
  node [shape=record, fontname=Helvetica, fontsize=10];
  b [ label="class B" URL="\ref B"];
  c [ label="class C" URL="\ref C"];
  b -> c [ arrowhead="open", style="dashed" ];
}
\enddot

\msc
Sender,Receiver;
Sender->Receiver [label="Command()", URL="\ref Receiver::Command()"];
Sender<-Receiver [label="Ack()", URL="\ref Ack()", ID="1"];
\endmsc

\startuml
!include Class.iuml

class object {
}
class object2 {
}

object <|-- object2
object *- object3
\enduml

\f$ a = \sqrt{c^2 - b^2} \f$
\f[ I = \frac{U}{R} \f]
\f$ c = \sqrt{a^2 + b^2} \f$.
\f[ I = \frac{U}{R} \f].


\bugtracker

*
* \author  gernot
* \date    2017-04-29
* \version 1.0
**********************************************************************/
void Test1( int param )
{  
  @ref secShaderSSAOIntro
  \ref secShaderCodeSnippetserRangeCheck
  @ref unknown_reference
  \ref unknown_reference
  \image html refl5_parallax_exp1.jpg
  @image html rim_lighting.svg
  \image html not_existing
  @image html not_existing
  \brief
  @brief
  \unknown_command
  @unknown_command
}
#pragma endregion

/* \brief no doxygen comment */
/** \brief doxygen comment */
/*! \brief doxygen comment */
// \brief no doxygen comment
/// \brief doxygen comment
//! \brief doxygen comment


/******************************************************************//**
* \brief   
* 
* \author  gernot
* \date    2017-05-14
* \version 1.0
**********************************************************************/
void Test2( int param )
{
}



/*XEOMETRIC********************************************************//**
* @brief Test Elite Extensions
*
* @note This comment is for demonstrating syntax highlighting.
*
* @attention A doxygen line comment has to start 
*            either with     //!
*            or              ///
*
* @attention A doxygen comment blocked has to be started 
*            either by       /*!  .....  
*            orby            /**  .....  
* 
* @author  
* @date    2018-07-30
* @version 1.0
**********************************************************************/
void Foo(void)
{

}


/*XEOMETRIC********************************************************//**
* @brief   Test Elite Extensions
*
* @image html isect_line_line_2d.png
* 
* @author  
* @date    2018-07-30
* @version 1.0
**********************************************************************/
void Foo(void)
{

}



/*XEOMETRIC********************************************************//**
* @brief   Test Elite Extensions

\dot
digraph example {
  node [shape=record, fontname=Helvetica, fontsize=10];
  b [ label="class B" URL="\ref B"];
  c [ label="class C" URL="\ref C"];
  b -> c [ arrowhead="open", style="dashed" ];
}
\enddot
 
* @author  
* @date    2018-07-30
* @version 1.0
**********************************************************************/
void Foo(void)
{

}




/*XEOMETRIC********************************************************//**
* @brief   Test Elite Extensions
*
* \dotfile uml_example.dot
*
* @author  
* @date    2018-07-30
* @version 1.0
**********************************************************************/
void Foo(void)
{

}



/*XEOMETRIC********************************************************//**
* @brief   Test Elite Extensions

\msc
Sender,Receiver;
Sender->Receiver [label="Command()", URL="\ref Receiver::Command()"];
Sender<-Receiver [label="Ack()", URL="\ref Ack()", ID="1"];
\endmsc
 
* @author  
* @date    2018-07-30
* @version 1.0
**********************************************************************/
void Foo(void)
{

}


/*XEOMETRIC********************************************************//**
* @brief   Test Elite Extensions
*
* \mscfile msc_example_1.msc
*
* @author  
* @date    2018-07-30
* @version 1.0
**********************************************************************/
void Foo(void)
{

}



/*XEOMETRIC********************************************************//**
* @brief   Test Elite Extensions
*
\F$
f_r( \omega_i, \, \omega_o ) \; = \; \displaystyle
\frac{ \mathrm{d} L_o(\omega_o) }{ \mathrm{d} E_i(\omega _i) } \; = \;
\frac { \mathrm{d} L_o( \omega_o ) }{ L_i( \omega_i ) \cdot \cos\theta_i \cdot \mathrm{d} \omega_i }
\F$
*
* @author  
* @date    2018-07-30
* @version 1.0
**********************************************************************/
void Foo(void)
{

}









































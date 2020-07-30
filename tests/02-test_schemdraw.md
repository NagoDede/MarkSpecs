# Schemdraw Implementation
This section contains some exemples and clarification about the use of Schemdraw in Markspecs.  
Schemdraw is Python library available on bitbucket at the following address: https://bitbucket.org/cdelker/schemdraw/src/master/  
Description and syntax are available on https://schemdraw.readthedocs.io/en/latest/index.html  
This last contains several exemples and we will use them as integratuib test means.

## Some considerations
We take credit of the Markspecs integration to remove the Python descriptions around the Schemdraw syntax.  
Markspecs builds a default drawing (nammed __d__) and there is no need to refer to the drawing object to add elements (no need to write d.add(...)). 
User can continue to create drawing by using the dedicated keywords "Drawing:  _id-of-drawing_ _{atttributes}_ " and "EndDrawing" to close the definition of a drawing.
You can refer to the _Infinite Transmission Line_ example to see these keywords in actions.  
The Markspecs integration of Schemdraw also allows the use of Python command in the block. Alseo, refer to the _Infinite Transmission Line_ example to see this point in action.

## Attributes definition
Markspecs supports the following attributes for the definition of the Drawing object.  
When the attributes are set to the declaration of the Schemdraw block, they will apply on the default (master) Drawing __d__.
Else, if you define a specific Drawing, you can define new attributes to this Drawing after the creation of the Drawing by the "Drawing:" key code.  
Supported attributes are unit, inches_per_unit, lblofst, fontsize, font, color, lw, ls, fill. Refer to https://schemdraw.readthedocs.io/en/latest/usage/classes.html#schemdraw.Drawing to have more information about them and how to use them.

## Schemdraw schematics.
### Simple schematic
This is a very basic schematic. It contains only a resistor, a source and a capacitor.
The Python code should be (the library references are not set):
```python
d.add(elm.Resistor(d='right', label='1$\Omega$'))
d.add(elm.Capacitor(d='down', label='10$\mu$F'))
d.add(elm.Line(d='left'))
d.add(elm.SourceSin(d='up', label='10V'))
d.draw()
```
In Markspecs, the equivalent code is
<pre>
```schemdraw
elm.Resistor(d='right', label='1$\Omega$')
elm.Capacitor(d='down', label='10$\mu$F')
elm.Line(d='left')
elm.SourceSin(d='up', label='10V')
```
</pre>
and it will generate the following diagram.
```schemdraw
elm.Resistor(d='right', label='1$\Omega$')
elm.Capacitor(d='down', label='10$\mu$F')
elm.Line(d='left')
elm.SourceSin(d='up', label='10V')
```
Note that the definition is more clear thanks the removal of the reference _d.add_.

### 741 Opamp Internal Schematic
This is a complexe schematic, it is build on the 741 Opamp Internal Schematic.  
Note the use of the push and pop commands and the allocation and use of some specific elements (like Q1, Q3,...).

```schemdraw {fontsize=12 unit=2.5}
Q1 = elm.BjtNpn(label='Q1', lftlabel='+IN')
Q3 = elm.BjtPnp('l', xy=Q1.emitter, anchor='emitter', lftlabel='Q3', flip=True)
elm.Line('d', xy=Q3.collector)
elm.Dot
push
elm.Line('r', l=d.unit/4)
Q7 = elm.BjtNpn(anchor='base', label='Q7')
pop
elm.Line('d', l=d.unit*1.25)
Q5 = elm.BjtNpn('l', anchor='collector', flip=True, lftlabel='Q5')
elm.Line('l', xy=Q5.emitter, l=d.unit/2, lftlabel='OFST\nNULL', move_cur=False)
elm.Resistor('d', xy=Q5.emitter, label='R1\n1K')
elm.Line('r', l=d.unit*.75)
elm.Dot
R3 = elm.Resistor('u', label='R3\n50K')
elm.Line(toy=Q5.base)
elm.Dot
push
elm.Line('l', to=Q5.base)
elm.Line('d', xy=Q7.emitter, toy=Q5.base)
elm.DOT
pop
elm.Line('right', l=d.unit/4)
Q6 = elm.BjtNpn(anchor='base', label='Q6')
elm.Line(xy=Q6.emitter, l=d.unit/3, rgtlabel='\nOFST\nNULL', move_cur=False)
elm.Resistor('d', xy=Q6.emitter, label='R2\n1K')
elm.Dot

elm.Line('u', xy=Q6.collector, toy=Q3.collector)
Q4 = elm.BjtPnp('r', anchor='collector', label='Q4')
elm.Line('l', xy=Q4.base, tox=Q3.base)
elm.Line('u', xy=Q4.emitter, toy=Q1.emitter)
Q2 = elm.BjtNpn('l', anchor='emitter', flip=True, lftlabel='Q2', rgtlabel='$-$IN')
elm.Line('u', xy=Q2.collector, l=d.unit/3)
elm.Dot
Q8 = elm.BjtPnp('l', lftlabel='Q8', anchor='base', flip=True)
elm.Line('d', xy=Q8.collector, toy=Q2.collector)
elm.Dot
elm.Line('l', xy=Q2.collector, tox=Q1.collector)
elm.Line('u', xy=Q8.emitter, l=d.unit/4)
top = elm.Line('l', tox=Q7.collector)
elm.Line('d', toy=Q7.collector)
elm.Line('r', xy=top.start, l=d.unit*2)
elm.Line('d', l=d.unit/4)
Q9 = elm.BjtPnp('r', anchor='emitter', label='Q9', lblofst=-.1)
elm.Line('l', xy=Q9.base, tox=Q8.base)
elm.Dot(xy=Q4.base)
elm.Line('d', xy=Q4.base, l=d.unit/2)
elm.Line('r', tox=Q9.collector)
elm.Dot
elm.Line('d', xy=Q9.collector, toy=Q6.collector)
Q10 = elm.BjtNpn('l', anchor='collector', flip=True, lftlabel='Q10')
elm.Resistor('d', xy=Q10.emitter, toy=R3.start, label='R4\n5K')
elm.Dot
Q11 = elm.BjtNpn('r', xy=Q10.base, anchor='base', label='Q11')
elm.Dot(xy=Q11.base)
elm.Line('u', l=d.unit/2)
elm.Line('r', tox=Q11.collector)
elm.Dot
elm.Line('d', xy=Q11.emitter, toy=R3.start)
elm.Dot
elm.Line('u', xy=Q11.collector, l=d.unit*2)
elm.Resistor(toy=Q9.collector, botlabel='R5\n39K')
Q12 = elm.BjtPnp('l', anchor='collector', flip=True, lftlabel='Q12', lblofst=-.1)
elm.Line('u', xy=Q12.emitter, l=d.unit/4)
elm.Dot
elm.Line('l', tox=Q9.emitter)
elm.Dot
elm.Line('r', xy=Q12.base, l=d.unit/4)
elm.Dot
push
elm.Line('d', toy=Q12.collector)
elm.Line('l', tox=Q12.collector)
elm.Dot
pop
elm.Line('r', l=d.unit*1.5)
Q13 = elm.BjtPnp(anchor='base', label='Q13')
elm.Line('u', l=d.unit/4)
elm.Dot
elm.Line('l', tox=Q12.emitter)
K = elm.Line('d', xy=Q13.collector, l=d.unit/5)
elm.Dot
elm.Line('d')
Q16 = elm.BjtNpn('r', anchor='collector', label='Q16', lblofst=-.1)
elm.Line('l', xy=Q16.base, l=d.unit/3)
elm.Dot
R7 = elm.Resistor('u', toy=K.end, label='R7\n4.5K')
elm.Dot
elm.Line('r', tox=Q13.collector, move_cur=False)
R8 = elm.Resistor('d', xy=R7.start, label='R8\n7.5K')
elm.Dot
elm.Line('r', tox=Q16.emitter)
J = elm.Dot
elm.Line('u', toy=Q16.emitter)
Q15 = elm.BjtNpn('r', anchor='collector', xy=R8.end, label='Q15')
elm.Line('l', xy=Q15.base, l=d.unit/2)
elm.Dot
C1 = elm.Capacitor('u', toy=R7.end, label='C1\n30pF')
elm.Line('r', tox=Q13.collector)
elm.Line('l', xy=C1.start, tox=Q6.collector)
elm.Dot
elm.Line('d', xy=J.center, l=d.unit/2)
Q19 = elm.BjtNpn('r', anchor='collector', label='Q19')
elm.Line('l', xy=Q19.base, tox=Q15.emitter)
elm.Dot
elm.Line('u', toy=Q15.emitter, move_cur=False)
elm.Line('d', xy=Q19.emitter, l=d.unit/4)
elm.Dot
elm.Line('left')
Q22 = elm.BjtNpn('l', anchor='base', flip=True, lftlabel='Q22')
elm.Line('u', xy=Q22.collector, toy=Q15.base)
elm.Dot
elm.Line('d', xy=Q22.emitter, toy=R3.start)
elm.Dot
elm.Line('l', tox=R3.start, move_cur=False)
elm.Line('r', tox=Q15.emitter)
elm.Dot
push
elm.Resistor('u', label='R12\n50K')
elm.Line(toy=Q19.base)
pop
elm.Line(tox=Q19.emitter)
elm.Dot
R11 = elm.Resistor('u', label='R11\n50')
elm.Line(toy=Q19.emitter)

elm.Line('u', xy=Q13.emitter, l=d.unit/4)
elm.Line('r', l=d.unit*1.5)
elm.Dot
elm.Line(l=d.unit/4, rgtlabel='V+', move_cur=False)
elm.Line('d', l=d.unit*.75)
Q14 = elm.BjtNpn('r', anchor='collector', label='Q14')
elm.Line('l', xy=Q14.base, l=d.unit/2)
push
elm.Dot
elm.Line('d', l=d.unit/2)
Q17 = elm.BjtNpn('l', anchor='collector', flip=True, lftlabel='Q17', lblofst=-.1)
elm.Line('r', xy=Q17.base, tox=Q14.emitter)
elm.Dot
J = elm.Line('u', toy=Q14.emitter)
pop
elm.Line(tox=Q13.collector)
elm.Dot
elm.Resistor('d', xy=J.start, label='R9\n25')
elm.Dot
push
elm.Line('l', tox=Q17.emitter)
elm.Line('u', toy=Q17.emitter)
pop
elm.Line('d', l=d.unit/4)
elm.Dot
elm.Line('r', l=d.unit/4, rgtlabel='OUT', move_cur=False)
elm.Resistor('d', label='R10\n50')
Q20 = elm.BjtPnp(d='r', anchor='emitter', label='Q20')
elm.Line('l', xy=Q20.base, l=d.unit/2)
elm.Line('u', toy=Q15.collector)
elm.Line('l', tox=Q15.collector)
elm.Dot
elm.Line('d', xy=Q20.collector, toy=R3.start)
elm.Dot
elm.Line('r', l=d.unit/4, rgtlabel='V-', move_cur=False)
elm.Line('l', tox=R11.start)
```
### Loop Current
The next exemple, shows how to use _loopI_ method (Drawing.loopI()) to create _Loop Current_ on your schematic.
```schemdraw {unit=5}
V1 = elm.SourceV(label='$20V$')
R1 = elm.Resistor(d='right', label='400$\Omega$')
elm.Dot()
push
R2 = elm.Resistor(d='down', botlabel='100$\Omega$', lblrotate=True)
elm.Dot()
pop
L1 = elm.Line()
I1 = elm.SourceI(d='down', botlabel='1A')
L2 = elm.Line(d='left', tox=V1.start)
loopI([R1,R2,L2,V1], '$I_1$', pad=1.25)
loopI([R1,I1,L2,R2], '$I_2$', pad=1.25)  # Use R1 as top element for both so they get the same height
```

### AC loop analysis
This case presents the _labelI_ mehod (Drawing.labelI) for an _AC Loop Analysis_.
```schemdraw
I1 = elm.SourceI(label=r'$5\angle 0^{\circ}$A')
elm.Dot()
push
elm.Capacitor('right', label=r'$-j3\Omega$')
elm.Dot()
push
elm.Inductor('down', label=r'$j2\Omega$')
elm.Dot()
pop
elm.Resistor('right', label=r'$5\Omega$')
elm.Dot()
V1 = elm.SourceV('down', reverse=True, botlabel=r'$5\angle -90^{\circ}$V')
elm.Line('left', tox=I1.start)
pop
elm.Line('up', l=d.unit*.8)
L1 = elm.Inductor('right', label=r'$j3\Omega$', tox=V1.start)
elm.Line('down', l=d.unit*.8)
l = labelI(L1, '$i_g$', top=False)
```

### Power Supply
The example shows how to use the attributes _inches_per_unit_ and _unit_. It also introduce the _Here_ methode, which is not documented in the API (d.here = [d.here[0]-d.unit, d.here[1]]).  
This is a good example to show how to refer to the default Diagram _d_.
```schemdraw {inches_per_unit=.5 unit=3}
D1 = elm.Diode(theta=-45)
elm.Dot
D2 = elm.Diode(theta=225, reverse=True)
elm.Dot
D3 = elm.Diode(theta=135, reverse=True)
elm.Dot
D4 = elm.Diode(theta=45)
elm.Dot

elm.Line('left', xy=D3.end, l=d.unit/2)
elm.Dot(open=True)
G = elm.Gap('up', toy=D1.start, label=['–', 'AC IN', '+'])
elm.Line('left', xy=D4.end, tox=G.start)
elm.Dot(open=True)

top = elm.Line('right', xy=D2.end, l=d.unit*3)
Q2 = elm.BjtNpn('up', circle=True, anchor='collector', label='Q2\n2n3055')
elm.Line('down', xy=Q2.base, l=d.unit/2)
Q2b = elm.Dot
elm.Line('left', l=d.unit/3)
Q1 = elm.BjtNpn('up', circle=True, anchor='emitter', label='Q1\n    2n3054')
elm.Line('up', xy=Q1.collector, toy=top.center)
elm.Dot

elm.Line('down', xy=Q1.base, l=d.unit/2)
elm.Dot
elm.Zener('down', reverse=True, botlabel='D2\n500mA')
elm.Dot
G = elm.Ground()
elm.Line('left')
elm.Dot
elm.Capacitor('up', polar=True, botlabel='C2\n100$\mu$F\n50V', reverse=True)
elm.Dot
push
elm.Line('right')
pop
elm.Resistor('up', toy=top.end, botlabel='R1\n2.2K\n50V')
elm.Dot

d.here = [d.here[0]-d.unit, d.here[1]]

elm.Dot
elm.Capacitor('down', polar=True, toy=G.start, label='C1\n 1000$\mu$F\n50V', flip=True)
elm.Dot
elm.Line('left', xy=G.start, tox=D4.start)
elm.Line('up', toy=D4.start)

elm.Resistor('right', xy=Q2b.center, label='R2', botlabel='56$\Omega$ 1W')
elm.Dot
push
elm.Line('up', toy=top.start)
elm.Dot
elm.Line('left', tox=Q2.emitter)
pop
elm.Capacitor('down', polar=True, toy=G.start, botlabel='C3\n470$\mu$F\n50V')
elm.Dot
elm.Line('left', tox=G.start, move_cur=False)
elm.Line('right')
elm.Dot
elm.Resistor('up', toy=top.center, botlabel='R3\n10K\n1W')
elm.Dot
elm.Line('left', move_cur=False)
elm.Line('right')
elm.Dot(open=True)
elm.Gap('down', toy=G.start, label=['+', '$V_{out}$', '–'])
elm.Dot(open=True)
elm.Line('left')
```

### Infinite Transmission Line
The _Infinite Transmission Line_, we can test the capacity to repeat elements thanks the introduction of Python code in the block and the definition of a new Schemdraw diagram (d1). 
User has to know that it is a potential security all, as it open doors to Python injection code. 
```schemdraw
Drawing: d1
elm.Resistor()
push
elm.Capacitor('down')
elm.Line('left')
pop
EndDrawing

for i in range(3):
    elm.ElementDrawing(d1)
push

elm.Line(l=d.unit/6)
elm.DotDotDot
elm.ElementDrawing(d1)
pop
d.here = [d.here[0], d.here[1]-d.unit]
elm.Line('right', l=d.unit/6)
elm.DotDotDot
```

### OpAmp pin labeling
```schemdraw {fontsize=12}
op = elm.Opamp(label='741', lblloc='center', lblofst=0)
elm.Line('left', xy=op.in1, l=.5)
elm.Line('down', l=d.unit/2)
elm.Ground
elm.Line('left', xy=op.in2, l=.5)
elm.Line('right', xy=op.out, l=.5, rgtlabel='$V_o$')
elm.Line('up', xy=op.vd, l=1, rgtlabel='$+V_s$')
trim = elm.Potentiometer('down', xy=op.n1, flip=True, zoom=.7)
elm.Line('right', tox=op.n1a)
elm.Line('up', to=op.n1a)
elm.Line('left', xy=trim.tap, tox=op.vs)
elm.Dot
push
elm.Line('down', l=d.unit/3)
elm.Ground
pop
elm.Line('up', toy=op.vs)
op.add_label('1', loc='n1', size=9, ofst=[-.1, -.25], align=('right', 'top'))
op.add_label('5', loc='n1a', size=9, ofst=[-.1, -.25], align=('right', 'top'))
op.add_label('4', loc='vs', size=9, ofst=[-.1, -.2], align=('right', 'top'))
op.add_label('7', loc='vd', size=9, ofst=[-.1, .2], align=('right', 'bottom'))
op.add_label('2', loc='in1', size=9, ofst=[-.1, .1], align=('right', 'bottom'))
op.add_label('3', loc='in2', size=9, ofst=[-.1, .1], align=('right', 'bottom'))
op.add_label('6', loc='out', size=9, ofst=[-.1, .1], align=('left', 'bottom'))
```
### Logic gates
```schemdraw {unit=.5}
X1 = logic.Xor
logic.Dot
A = logic.Dot(xy=X1.in1)
Ain = logic.Line('left', l=d.unit*2, lftlabel='$A$')
logic.Line('left', xy=X1.in2)
B = logic.Dot
logic.Line('left', lftlabel='$B$')

logic.Line('right', xy=X1.out, l=d.unit)
X2 = logic.Xor(anchor='in1')
C = logic.Line('down', xy=X2.in2, l=d.unit*2)
push
logic.Dot(xy=C.center)
logic.Line('left', tox=Ain.end, lftlabel='$C_{in}$')
pop

A1 = logic.And('right', anchor='in1')
logic.Line('left', xy=A1.in2, tox=X1.out)
logic.Line('up', toy=X1.out)
A2 = logic.And('right', anchor='in1', xy=[A1.in1[0],A1.in2[1]-d.unit*2])
logic.Line('left', xy=A2.in1, tox=A.start)
logic.Line('up', toy=A.start)
logic.Line('left', xy=A2.in2, tox=B.start)
logic.Line('up', toy=B.start)

O1 = logic.Or('right', xy=[A1.out[0],(A1.out[1]+A2.out[1])/2], rgtlabel='$C_{out}$')
logic.Line('down', xy=A1.out, toy=O1.in1)
logic.Line('up', xy=A2.out, toy=O1.in2)
logic.Line('right', xy=X2.out, tox=O1.out, rgtlabel='$S$')
```


### Superheterodyne Receiver
```schemdraw {fontsize=12}
dsp.Antenna
dsp.Line('right', l=d.unit/4)
filt1 = dsp.Filter(response='bp', botlabel='RF filter\n#1', anchor='W', lblofst=.2, fill='thistle')
dsp.Line(xy=filt1.E, l=d.unit/4)
dsp.Amp(label='LNA', fill='lightblue')
dsp.Line(l=d.unit/4)
filt2 = dsp.Filter(response='bp', botlabel='RF filter\n#2', anchor='W', lblofst=.2, fill='thistle')
dsp.Line('right', xy=filt2.E, l=d.unit/3)
mix = dsp.Mixer(label='Mixer', fill='navajowhite')
dsp.Line('down', xy=mix.S, l=d.unit/3)
dsp.Oscillator('right', rgtlabel='Local\nOscillator', lblofst=.2, anchor='N', fill='navajowhite')
dsp.Line('right', xy=mix.E, l=d.unit/3)
filtIF = dsp.Filter(response='bp', anchor='W', botlabel='IF filter', lblofst=.2, fill='thistle')
dsp.Line('right', xy=filtIF.E, l=d.unit/4)
dsp.Amp(label='IF\namplifier', fill='lightblue')
dsp.Line(l=d.unit/4)
demod = dsp.Demod(anchor='W', botlabel='Demodulator', lblofst=.2, fill='navajowhite')
dsp.Arrow('right', xy=demod.E, l=d.unit/3)
```

### Styles
```schemdraw{inches_per_unit=.5 unit=3}
for i, color in enumerate(['red', 'orange', 'yellow', 'yellowgreen', 'green', 'blue', 'indigo', 'violet']):
    d.add(elm.Resistor(label='R{}'.format(i), theta=45*i, color=color))
```

### Integrated Circuits
```schemdraw
T = elm.Ic(pins=[elm.IcPin(name='TRG', side='left', pin='2'), elm.IcPin(name='THR', side='left', pin='6'),elm.IcPin(name='DIS', side='left', pin='7'),elm.IcPin(name='CTL', side='right', pin='5'),elm.IcPin(name='OUT', side='right', pin='3'),elm.IcPin(name='RST', side='top', pin='4'),elm.IcPin(name='Vcc', side='top', pin='8'),elm.IcPin(name='GND', side='bot', pin='1'),],edgepadW=.5,edgepadH=1,pinspacing=2,leadlen=1,label='555')

BOT = elm.Ground(xy=T.GND)
elm.Dot
elm.Resistor(endpts=[T.DIS, T.THR], label='Rb')
elm.Resistor('u', xy=T.DIS, label='Ra', rgtlabel='+Vcc')
elm.Line(endpts=[T.THR, T.TRG])
elm.Capacitor('d', xy=T.TRG, toy=BOT.start, label='C', l=d.unit/2)
elm.Line('r', tox=BOT.start)
elm.Capacitor('d', xy=T.CTL, toy=BOT.start, botlabel='.01$\mu$F')
elm.Dot(xy=T.DIS)
elm.Dot(xy=T.THR)
elm.Dot(xy=T.TRG)
elm.Line(endpts=[T.RST,T.Vcc])
elm.Dot
elm.Line('u', l=d.unit/4, rgtlabel='+Vcc')
elm.Resistor('r', xy=T.OUT, label='330')
elm.LED(flip=True, d='down', toy=BOT.start)
elm.Line('l', tox=BOT.start)
```
This exemple shows that it is possible to define Python class in a Schemdraw block.
The integration can manage some line breaks in the definition. Line breaks are possible only under array definition outside element definition. If you experiment some troubles with the definition, try to define the tables on a single row.
```schemdraw {fontsize=11 inches_per_unit=.4}
class Atmega328(elm.Ic):
    def __init__(self, *args, **kwargs):
#Here line breaks are allowed
        pins=[elm.IcPin(name='PD0', pin='2', side='r', slot='1/22'),
              elm.IcPin(name='PD1', pin='3', side='r', slot='2/22'),
              elm.IcPin(name='PD2', pin='4', side='r', slot='3/22'),
              elm.IcPin(name='PD3', pin='5', side='r', slot='4/22'),
              elm.IcPin(name='PD4', pin='6', side='r', slot='5/22'),
              elm.IcPin(name='PD5', pin='11', side='r', slot='6/22'),
              elm.IcPin(name='PD6', pin='12', side='r', slot='7/22'),
              elm.IcPin(name='PD7', pin='13', side='r', slot='8/22'),
              elm.IcPin(name='PC0', pin='23', side='r', slot='10/22'),
              elm.IcPin(name='PC1', pin='24', side='r', slot='11/22'),
              elm.IcPin(name='PC2', pin='25', side='r', slot='12/22'),
              elm.IcPin(name='PC3', pin='26', side='r', slot='13/22'),
              elm.IcPin(name='PC4', pin='27', side='r', slot='14/22'),
              elm.IcPin(name='PC5', pin='28', side='r', slot='15/22'),
              elm.IcPin(name='PB0', pin='14', side='r', slot='17/22'),
              elm.IcPin(name='PB1', pin='15', side='r', slot='18/22'),
              elm.IcPin(name='PB2', pin='16', side='r', slot='19/22'),
              elm.IcPin(name='PB3', pin='17', side='r', slot='20/22'),
              elm.IcPin(name='PB4', pin='18', side='r', slot='21/22'),
              elm.IcPin(name='PB5', pin='19', side='r', slot='22/22'),
              elm.IcPin(name='RESET', side='l', slot='22/22', invert=True, pin='1'),
              elm.IcPin(name='XTAL2', side='l', slot='19/22', pin='10'),
              elm.IcPin(name='XTAL1', side='l', slot='17/22', pin='9'),
              elm.IcPin(name='AREF', side='l', slot='15/22', pin='21'),
              elm.IcPin(name='AVCC', side='l', slot='14/22', pin='20'),
              elm.IcPin(name='AGND', side='l', slot='13/22', pin='22'),
              elm.IcPin(name='VCC', side='l', slot='11/22', pin='7'),
              elm.IcPin(name='GND', side='l', slot='10/22', pin='8')]
        super().__init__(pins=pins, w=5, plblofst=.05, botlabel='ATMEGA328', **kwargs)

Q1 = d.add(Atmega328()) #As the Atmega is defined through a class, need to add it to the diagram manually.

#Here,line break is not supported.
JP4 = elm.Header(rows=10, shownumber=True, flip=True, at=[Q1.PB5[0]+4, Q1.PB5[1]+1], anchor='p6', label='JP4', fontsize=10, pinsright=['D8', 'D9', 'D10', 'D11', 'D12', 'D13', '', '', '', ''], pinalignright='center')
JP3 = elm.Header(rows=6, shownumber=True, flip=True, at=[Q1.PC5[0]+4, Q1.PC5[1]], anchor='p6', label='JP3', fontsize=10, pinsright=['A0', 'A1', 'A2', 'A3', 'A4', 'A5'], pinalignright='center')
JP2 = elm.Header(rows=8, shownumber=True, flip=True, at=[Q1.PD7[0]+3, Q1.PD7[1]], anchor='p8', label='JP2', fontsize=10, pinsright=['D0', 'D1', 'D2', 'D3', 'D4', 'D5', 'D6', 'D7'], pinalignright='center')

elm.OrthoLines(at=Q1.PB5, to=JP4.p6, n=6)
elm.OrthoLines(at=Q1.PC5, to=JP3.p6, n=6)
elm.OrthoLines(at=Q1.PD7, to=JP2.p8, n=8)

elm.Line('l', at=JP4.p7, l=.9, lftlabel='GND')
elm.Line('l', at=JP4.p8, l=.9, lftlabel='AREF')
elm.Line('l', at=JP4.p9, l=.9, lftlabel='AD4/SDA')
elm.Line('l', at=JP4.p10, l=.9, lftlabel='AD5/SCL')

JP1 = elm.Header('r', at=[Q1.PD0[0]+4, Q1.PD0[1]-2], rows=6, anchor='p1', shownumber=True, pinsright=['VCC', 'RXD', 'TXD', 'DTR', 'RTS', 'GND'], pinalignright='center')
elm.Line('l', at=JP1.p1, l=d.unit/2)
elm.Vdd(label='+5V')
elm.Line('l', at=JP1.p2, l=d.unit)
elm.Line('u', toy=Q1.PD0)
elm.Dot
elm.Line('l', at=JP1.p3, l=d.unit+0.6)
elm.Line('u', toy=Q1.PD1)
elm.Dot
elm.Line('l', at=JP1.p6, l=d.unit/2)
elm.Ground

elm.Line('l', at=Q1.XTAL2, l=d.unit*2)
elm.Dot
push
elm.Capacitor('l', zoom=.75, l=d.unit/2)
elm.Line('d', toy=Q1.XTAL1)
elm.Dot
elm.Ground
elm.Capacitor('r', zoom=.75, l=d.unit/2)
elm.Dot
pop
elm.Crystal('d', botlabel='16MHz', toy=Q1.XTAL1)
elm.Line('r', tox=Q1.XTAL1)

elm.Line('l', at=Q1.AREF, l=d.unit/3, lftlabel='AREF')
elm.Line('l', at=Q1.AVCC, l=1.5*d.unit)
elm.Vdd(label='+5V')
elm.Dot
elm.Line('d', toy=Q1.VCC)
elm.Dot
elm.Line('r', tox=Q1.VCC, move_cur=False)
elm.Capacitor('d', label='100n')
GND = elm.Ground

elm.Line('l', at=Q1.AGND)
elm.Line('d', toy=Q1.GND)
elm.Dot
elm.Line('r', tox=Q1.GND, move_cur=False)
elm.Line('d', toy=GND.xy)
elm.Line('l', tox=GND.xy)
elm.Dot

elm.Line('l', at=Q1.RESET)
elm.Dot
push
elm.RBox('u', label='10K')
elm.Vdd(label='+5V')
pop
elm.Line('l')
push
elm.Dot
RST = elm.Button('up', label='Reset')
elm.Line('l', l=d.unit/2)
elm.Ground
pop

elm.Capacitor('l', at=JP1.p4, botlabel='100n')
elm.Line('l', tox=RST.start[0]-2)
elm.Line('u', toy=Q1.RESET)
elm.Line('r', tox=RST.start)
```
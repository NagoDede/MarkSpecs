# Test pour mocodo
This is a test for mocodo extension on Markdig

```mocodo {colors=ocean image_format="svg" shapes=copperplate relations="html_verbose" }
DF, 11 Élève, 1N Classe
Classe: Num. classe, Num. salle
Faire Cours, 1N Classe, 1N Prof: Vol. horaire
Catégorie: Code catégorie, Nom catégorie

Élève: Num. élève, Nom élève
Noter, 1N Élève, 0N Prof, 0N Matière, 1N Date: Note
Prof: Num. prof, Nom prof
Relever, 0N Catégorie, 11 Prof

Date: Date
Matière: Libellé matière
Enseigner, 11 Prof, 1N Matière
```

Here we have a simple Schemdraw schematic.
```schemdraw
elm.Resistor(d='right', label='1$\Omega$')
elm.Capacitor(d='down', label='10$\mu$F')
elm.Line(d='left')
elm.SourceSin(d='up', label='10V')
```

This is a complexe schematic
```schemdraw
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
push
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
Q14 = elm.BjtNpn('r', anchr='collector', label='Q14')
elm.Line('l', xy=Q14.base, l=d.unit/2)
push
elm.Dot
elm.Line('d', l=d.unit/2)
Q17 = elm.BjtNpn('l', anchor='collector', flip=True, lftlabel='Q17', lblofst=-.1)
elm.Line('r', xy=Q17.base, tox=Q14.emitter)
elm.Dot
J = elm.Line('u', toy=Q14.emitter)
push
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

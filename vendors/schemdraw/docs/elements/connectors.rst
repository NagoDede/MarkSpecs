Connectors
==========

.. jupyter-execute::
    :hide-code:
    
    from functools import partial
    import schemdraw
    from schemdraw import elements as elm

All connectors are defined with a default pin spacing of 0.6, matching the default pin spacing of the :py:class:`schemdraw.elements.intcircuits.Ic` class, for easy connection of multiple signals.


Headers
^^^^^^^

A :py:class:`schemdraw.elements.connectors.Header` is a generic Header block with any number of rows and columns. It can have round, square, or screw-head connection points.

.. class:: schemdraw.elements.connectors.Header(**kwargs)

    Generic Header connector element

    :param rows: Number of rows [4]
    :type rows: int
    :param cols: Number of columns. Pin numbering requires 1 or 2 columns. [1]
    :type cols: int
    :param style: Connector style, 'round', 'square', or 'screw'
    :type style: string
    :param numbering: Pin numbering order. 'lr' for left-to-right numbering, 'ud' for up-down numbering, or 'ccw' for counter-clockwise integrated-circuit style numbering. Pin 1 is always at the top-left corner, unless `flip` parameter is also set.
    :type numbering: string
    :param shownumber: Draw pin numbers outside the header [False]
    :type shownumber: bool
    :param pinsleft: List of pin labels for left side
    :type pinsleft: list
    :param pinsright: List of pin labels for right side
    :type pinsright: list
    :param pinalignleft: Vertical alignment for pins on left side ('center', 'top', 'bottom')
    :type pinalignleft: string
    :param pinalignright: Vertical alignment for pins on right side ('center', 'top', 'bottom')
    :param pinfontsizeleft: Font size for pin labels on left
    :type pinfontsizeleft: float
    :param pinfontsizeright: Font size for pin labels on right
    :type pinfontsizeright: float
    :param pinspacing: Distance between pins [0.6]
    :type pinspacing: float
    :param edge: Distance between header edge and first pin row/column [0.3]
    :type edge: float
    :param pinfill: Color to fill pin circles
    :type pinfill: string


.. jupyter-execute::
    :hide-code:
    
    def drawElements(elmlist, cols=3, dx=8, dy=2):
        d = schemdraw.Drawing(fontsize=12)
        for i, e in enumerate(elmlist):
            y = i//cols*-dy
            x = (i%cols) * dx

            name = type(e()).__name__
            if hasattr(e, 'keywords'):  # partials have keywords attribute
                args = ', '.join(['{}={}'.format(k, v) for k, v in e.keywords.items()])
                name = '{}({})'.format(name, args)
            eplaced = d.add(e, d='right', xy=[x, y])
            eplaced.add_label(name, loc='rgt', align=('left', 'center'))
        return d

    elmlist = [elm.Header,
           partial(elm.Header, shownumber=True),
           partial(elm.Header, rows=3, cols=2),
           partial(elm.Header, style='square'),
           partial(elm.Header, style='screw'),
           partial(elm.Header, pinsleft=['A', 'B', 'C', 'D'], pinalignleft='center')]
    drawElements(elmlist, cols=2, dy=4)
    
    
Header pins are given anchor names `p1`, `p2`, etc.    
Pin number labels and anchor names can be ordered left-to-right (`lr`), up-to-down (`ud`), or counterclockwise (`ccw`) like a traditional IC, depending on the `numbering` argument.
The `flip` argument can be set True to put pin 1 at the bottom.

.. jupyter-execute::
    :hide-code:
    
    d = schemdraw.Drawing()
    d.add(elm.Header(shownumber=True, cols=2, numbering='lr', label="lr"))
    d.add(elm.Header(at=[3, 0], shownumber=True, cols=2, numbering='ud', label="ud"))
    d.add(elm.Header(at=[6, 0], shownumber=True, cols=2, numbering='ccw', label="ccw"))
    d.draw()

A Jumper element is also defined, as a simple rectangle, for easy placing onto a header.

.. jupyter-execute::
    :hide-code:

    d = schemdraw.Drawing()

.. jupyter-execute::
    :hide-output:

    J = d.add(elm.Header(cols=2, style='square'))
    d.add(elm.Jumper(at=J.p3, fill='lightgray'))

.. jupyter-execute::
    :hide-code:

    d.draw()
    

D-Sub Connectors
^^^^^^^^^^^^^^^^

Both DB9 and DB25 subminiature connectors are defined, with anchors `p1` through `p9` or `p25`.

.. jupyter-execute::
    :hide-code:

    d = schemdraw.Drawing(fontsize=12)
    d.add(elm.DB9(label='DB9'))
    d.add(elm.DB9(at=[3, 0], number=True, label='DB9(number=True)'))
    d.add(elm.DB25(at=[6, 0], label='DB25'))
    d.draw()


Multiple Lines
^^^^^^^^^^^^^^

The :py:class:`schemdraw.elements.connectors.RightLines` and :py:class:`schemdraw.elements.connectors.OrthoLines` elements are useful for connecting multiple pins of an integrated circuit or header all at once. Both need an `at` and `to` location specified, along with the `n` parameter for setting the number of lines to draw.

Use RightLines when the Headers are perpindicular to each other.

.. jupyter-execute::
    :hide-code:

    d = schemdraw.Drawing(fontsize=12)

.. jupyter-execute::
    :hide-output:

    D1 = d.add(elm.Ic(pins=[elm.IcPin(name='A', side='t', slot='1/4'),
                            elm.IcPin(name='B', side='t', slot='2/4'),
                            elm.IcPin(name='C', side='t', slot='3/4'),
                            elm.IcPin(name='D', side='t', slot='4/4')]))
    D2 = d.add(elm.Header(rows=4, at=[5,4]))
    d.add(elm.RightLines(at=D2.p1, to=D1.D, n=4, label='RightLines'))

.. jupyter-execute::
    :hide-code:

    d.draw()

OrthoLines draw a z-shaped orthogonal connection. Use OrthoLines when the Headers are parallel but vertically offset.
Use the `xstart` parameter, between 0 and 1, to specify the position where the first OrthoLine turns vertical.

.. jupyter-execute::
    :hide-code:

    d = schemdraw.Drawing(fontsize=12)

.. jupyter-execute::
    :hide-output:

    D1 = d.add(elm.Ic(pins=[elm.IcPin(name='A', side='r', slot='1/4'),
                            elm.IcPin(name='B', side='r', slot='2/4'),
                            elm.IcPin(name='C', side='r', slot='3/4'),
                            elm.IcPin(name='D', side='r', slot='4/4')]))
    D2 = d.add(elm.Header(rows=4, at=[7, -3]))
    d.add(elm.OrthoLines(at=D1.D, to=D2.p1, n=4, label='OrthoLines'))

.. jupyter-execute::
    :hide-code:

    d.draw()


Data Busses
^^^^^^^^^^^

Sometimes, multiple I/O pins to an integrated circuit are lumped together into a data bus.
The connections to a bus can be drawn using the :py:class:`schemdraw.elements.connectors.BusConnect` element, which takes `n` the number of data lines and an argument.
:py:class:`schemdraw.elements.connectors.BusLine` is simply a wider line used to extend the full bus to its destination.

BusConnect elements define anchors `start`, `end` on the endpoints of the wide bus line, and `p1`, `p2`, etc. for the individual signals.


.. jupyter-execute::
    :hide-code:

    d = schemdraw.Drawing()

.. jupyter-execute::
    :hide-output:

    J = d.add(elm.Header(rows=6))
    B = d.add(elm.BusConnect(n=6, at=J.p1))
    d.add(elm.BusLine('down', at=B.end, l=3))
    B2 = d.add(elm.BusConnect(n=6, anchor='start', reverse=True))
    d.add(elm.Header(rows=6, at=B2.p1, anchor='p1'))

.. jupyter-execute::
    :hide-code:

    d.draw()

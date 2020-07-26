''' One terminal element definitions '''

from ..segments import Segment
from .elements import Element
from .twoterm import resheight, gap


gndgap = 0.12
gnd_lead = 0.4


class Ground(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.segments.append(Segment(
            [[0, 0], [0, -gnd_lead], [-resheight, -gnd_lead],
             [resheight, -gnd_lead], gap, [-resheight*.7, -gndgap-gnd_lead],
             [resheight*.7, -gndgap-gnd_lead], gap,
             [-resheight*.2, -gndgap*2-gnd_lead],
             [resheight*.2, -gndgap*2-gnd_lead]]))
        self.params['theta'] = 0
        self.params['drop'] = [0, 0]
        self.anchors['start'] = [0, 0]
        self.anchors['center'] = [0, 0]
        self.anchors['end'] = [0, 0]


class GroundSignal(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self.segments.append(Segment(
            [[0, 0], [0, -gnd_lead], [-resheight, -gnd_lead], [0, -gnd_lead*2],
             [resheight, -gnd_lead], [0, -gnd_lead]]))
        self.params['drop'] = [0, 0]
        self.params['theta'] = 0
        self.anchors['start'] = [0, 0]
        self.anchors['center'] = [0, 0]
        self.anchors['end'] = [0, 0]


class GroundChassis(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        dx = resheight*.75
        dy = resheight
        self.segments.append(Segment(
            [[0, 0], [0, -gnd_lead], [-dx, -gnd_lead-dy]]))
        self.segments.append(Segment(
            [[0, -gnd_lead], [-dx, -gnd_lead], [-dx*2, -gnd_lead-dy]]))
        self.segments.append(Segment(
            [[0, -gnd_lead], [dx, -gnd_lead], [0, -gnd_lead-dy]]))
        self.params['drop'] = [0, 0]
        self.params['theta'] = 0
        self.anchors['start'] = [0, 0]
        self.anchors['center'] = [0, 0]
        self.anchors['end'] = [0, 0]


class Antenna(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        lead = 0.6
        h = 0.6
        w = 0.38
        self.segments.append(Segment(
            [[0, 0], [0, lead], [-w, lead+h], [w, lead+h], [0, lead]]))
        self.params['drop'] = [0, 0]
        self.params['theta'] = 0
        self.anchors['start'] = [0, 0]
        self.anchors['center'] = [0, 0]
        self.anchors['end'] = [0, 0]


class AntennaLoop(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        lead = 0.2
        h = 0.5
        self.segments.append(Segment(
            [[0, 0], [0, lead], [-h, h+lead], [lead/2, h*2+1.5*lead],
             [h+lead, h+lead], [lead, lead], [lead, 0]]))
        self.params['drop'] = [0, 0]
        self.params['theta'] = 0
        self.anchors['start'] = [0, 0]
        self.anchors['end'] = [lead, 0]


class AntennaLoop2(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        lead = .25
        h = 1
        x1 = -h/2-lead/2
        x2 = x1 + lead
        x3 = h/2+lead/2
        x4 = x3 + lead
        y1 = lead
        y2 = lead*2
        y3 = h+y2
        y4 = y3+lead
        self.segments.append(Segment(
            [[0, 0], [0, y1], [x1, y1], [x1, y3], [x3, y3], [x3, y2],
             [x2, y2], [x2, y4], [x4, y4], [x4, y1], [lead, y1], [lead, 0]]))
        self.params['drop'] = [lead, 0]
        self.params['theta'] = 0
        self.anchors['start'] = [0, 0]
        self.anchors['end'] = [lead, 0]


class Vss(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        dx = resheight*.75
        self.segments.append(Segment([[0, 0], [0, -gnd_lead]]))
        self.segments.append(Segment([[0, -gnd_lead], [-dx, -gnd_lead]]))
        self.segments.append(Segment([[0, -gnd_lead], [dx, -gnd_lead]]))
        self.theta = 0
        self.params['drop'] = [0, 0]
        self.params['theta'] = 0
        self.anchors['start'] = [0, 0]
        self.anchors['center'] = [0, 0]
        self.anchors['end'] = [0, 0]


class Vdd(Element):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        dx = resheight*.75
        self.segments.append(Segment([[0, 0], [0, gnd_lead]]))
        self.segments.append(Segment([[0, gnd_lead], [-dx, gnd_lead]]))
        self.segments.append(Segment([[0, gnd_lead], [dx, gnd_lead]]))
        self.theta = 0
        self.params['drop'] = [0, 0]
        self.params['theta'] = 0
        self.anchors['start'] = [0, 0]
        self.anchors['center'] = [0, 0]
        self.anchors['end'] = [0, 0]

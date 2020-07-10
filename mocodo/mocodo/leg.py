#!/usr/bin/env python
# encoding: utf-8

from __future__ import division

import sys
import re
from .dynamic import Dynamic

match_card = re.compile(r"(_11|..)([<>]?)\s*(?:\[(.+?)\])?").match

html_escape_table = {
    "&": "&amp;",
    '"': r'\\"',
    "'": r"\\'",
    ">": "&gt;",
    "<": "&lt;",
}

auto_correction = {
    "01": ["O1", "o1", "10", "1O", "1o"],
    "0N": ["ON", "oN", "NO", "No", "N0"],
    "0n": ["On", "on", "no", "nO", "n0"],
    "1N": ["N1"],
    "1n": ["n1"]
}
auto_correction = dict((v, k) for k in auto_correction for v in auto_correction[k])

def html_escape(text):
    return "".join(html_escape_table.get(c, c) for c in text)

class Leg:

    def __init__(self, association, card, entity_name, params):
        self.association = association
        self.may_identify = not entity_name.startswith("/")
        if not self.may_identify:
            entity_name = entity_name[1:]
        self.entity_name = entity_name
        (self.cards, self.arrow, self.annotation) = match_card(card).groups()
        self.underlined_card = False
        self.strengthen = self.cards == "_11"
        if self.strengthen:
            self.cards = "11"
            if params["strengthen_card"].startswith("_") and params["strengthen_card"].endswith("_"):
                self.underlined_card = True
                self.cardinalities = params["strengthen_card"][1:-1]
            else:
                self.cardinalities = params["strengthen_card"]
        else:
            self.cards = auto_correction.get(self.cards, self.cards)
            if self.cards.startswith("XX"):
                self.cardinalities = u"     "
            else:
                self.cardinalities = params["card_format"].format(min_card=self.cards[0], max_card=self.cards[1])
        if self.annotation:
            self.annotation = html_escape(self.annotation.replace("<<<protected-comma>>>", ",").replace("<<<protected-colon>>>", ":"))
        self.twist = False
        self.identifier = None
    
    def calculate_size(self, style, get_font_metrics):
        font = get_font_metrics(style["card_font"])
        self.h = font.get_pixel_height()
        self.w = font.get_pixel_width(self.cardinalities)
        self.style = style
    
    def set_spin_strategy(self, spin):
        self.spin = spin
        self.description = (self._curved_description if spin else self._straight_description)
        if self.identifier is None:
            self.identifier = ("%s,%s,%s" % (self.association.name, self.entity_name, self.spin) if spin else "%s,%s" % (self.association.name, self.entity_name))
    
    def _straight_description(self):
        result = []
        result.append({
                "key": "env",
                "env": [("ex", """cx[u"%s"]""" % self.entity.name), ("ey", """cy[u"%s"]""" % self.entity.name)],
            })
        result.append({
                "key": "stroke_color",
                "stroke_color": Dynamic("colors['leg_stroke_color']"),
            })
        result.append({
                "key": "stroke_depth",
                "stroke_depth": self.style["leg_stroke_depth"],
            })
        result.append({
                "key": "straight_leg",
                "ex": Dynamic("ex"),
                "ey": Dynamic("ey"),
                "ew": self.entity.w // 2,
                "eh": self.entity.h // 2,
                "ax": Dynamic("x"),
                "ay": Dynamic("y"),
                "aw": self.association.w // 2,
                "ah": self.association.h // 2,
                "cw": self.w,
                "ch": self.h,
                "stroke_depth": self.style["leg_stroke_depth"],
                "stroke_color": Dynamic("colors['leg_stroke_color']"),
            })
        result.append({
                "key": "straight_card",
                "text": self.cardinalities,
                "text_color": Dynamic("colors['card_text_color']"),
                "leg_identifier": "%s,%s" % (self.association.name, self.entity_name),
                "family": self.style["card_font"]["family"],
                "size": self.style["card_font"]["size"],
                "twist": self.twist,
        })
        if self.annotation:
            result[-1].update({
                "key": "straight_card_note",
                "annotation": self.annotation,
            })
        if self.underlined_card:
            result.append({
                    "key": "stroke_depth",
                    "stroke_depth": self.style["card_underline_depth"],
                })
            result.append({
                    "key": "stroke_color",
                    "stroke_color": Dynamic("colors['card_text_color']"),
                })
            result.append({
                    "key": "card_underline",
                    "w": self.w,
                    "skip": self.style["card_underline_skip_height"],
                })
        if self.arrow:
            result.extend([
                {
                    "key": "color",
                    "color": Dynamic("colors['leg_stroke_color']"),
                },
                {
                    "key": "stroke_depth",
                    "stroke_depth": 0,
                },
                {
                    "key": "straight_arrow",
                    "direction": self.arrow,
                    "leg_identifier": "%s,%s" % (self.association.name, self.entity_name),
                }
            ])
        return result

    def _curved_description(self):
        result = []
        result.append({
                "key": "env",
                "env": [("ex", """cx[u"%s"]""" % self.entity.name), ("ey", """cy[u"%s"]""" % self.entity.name)],
            })
        result.append({
                "key": "stroke_depth",
                "stroke_depth": self.style["leg_stroke_depth"],
            })
        result.append({
                "key": "stroke_color",
                "stroke_color": Dynamic("colors['leg_stroke_color']"),
            })
        result.append({
                "key": "curved_leg",
                "ex": Dynamic("ex"),
                "ey": Dynamic("ey"),
                "ew": self.entity.w // 2,
                "eh": self.entity.h // 2,
                "ax": Dynamic("x"),
                "ay": Dynamic("y"),
                "aw": self.association.w // 2,
                "ah": self.association.h // 2,
                "spin": self.spin,
                "cw": self.w,
                "ch": self.h,
                "stroke_depth": self.style["leg_stroke_depth"],
                "stroke_color": Dynamic("colors['leg_stroke_color']"),
            })
        result.append({
                "key": "curved_card",
                "text": self.cardinalities,
                "text_color": Dynamic("colors['card_text_color']"),
                "leg_identifier": self.identifier,
                "family": self.style["card_font"]["family"],
                "size": self.style["card_font"]["size"],
            })
        if self.annotation:
            result[-1].update({
                    "key": "curved_card_note",
                    "annotation": self.annotation,
                })
        if self.strengthen:
            result.append({
                    "key": "stroke_depth",
                    "stroke_depth": self.style["card_underline_depth"],
                })
            result.append({
                    "key": "stroke_color",
                    "stroke_color": Dynamic("colors['card_text_color']"),
                })
            result.append({
                    "key": "card_underline",
                    "w": self.w,
                    "skip": self.style["card_underline_skip_height"],
                })
        if self.arrow:
            result.extend([
                {
                    "key": "color",
                    "color": Dynamic("colors['leg_stroke_color']"),
                },
                {
                    "key": "stroke_depth",
                    "stroke_depth": 0,
                },
                {
                    "key": "curved_arrow",
                    "direction": self.arrow,
                    "leg_identifier": self.identifier,
                }
            ])
        return result


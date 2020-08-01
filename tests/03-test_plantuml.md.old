# This is a plantuml diagram

```plantuml
@startuml


autonumber
box "UnTrustedDomain" #Red
	database UnTrustedKeyStore as UKS
	control UnTrustedKeyManager as UKM
    boundary KeyLoader as KL
end box

box "TrustedDomain" #Green
	database PersistentStorage as KS
    control KeyManager as KM

end box


group In the Beginning...
    
    group RootKBPK    
        note over KM: A Unique random key is born
        KS --> KS: Root KBPK exists
    end
   
    group Storage in UnTrustedDomain 
        KM --> KM: Create Class N KPBK
        KS --> KM: RootKBPK 
        note over KM: Shorthand for create a KeyBlock with ClassKBPK N as key payload, and RootKBPK as KBPK
        UKM --> UKS: KeyBlock[ClassKBPK N]RootKBPK        
        note over UKS: Process is repeated for ClassKBPK 1,2,3...N 
    end
end 

@enduml
```

```plantuml
@startuml
actor actors
agent agents
artifact artifacts
boundary boundarys
card cards
cloud clouds
component component
control control
database database
entity entity
file file
folder folder
frame frame
interface  interface
node node
package package
queue queue
stack stack
rectangle rectangle
storage storage
usecase usecase
@enduml
```

This is the end of plantuml diagram

```plantuml
@startuml
left to right direction
actor "Food Critic" as fc
package Restaurant {
  usecase "Eat Food" as UC1
  usecase "Pay for Food" as UC2
  usecase "Drink" as UC3
}
fc --> UC1
fc --> UC2
fc --> UC3
@enduml
```

This is a NwDiag.
MarkSpecs will complete the description to add the "nwdiag{" at the beginning and  "}" at the end


```nwdiag
network Sample_front {
    address = "192.168.10.0/24";

    // define group
    group web {
      web01 [address = ".1"];
      web02 [address = ".2"];
    }
  }
  network Sample_back {
    address = "192.168.20.0/24";
    web01 [address = ".1"];
    web02 [address = ".2"];
    db01 [address = ".101"];
    db02 [address = ".102"];

    // define network using defined nodes
    group db {
      db01;
      db02;
    }
  }
```




```plantuml
@startuml

User -> (Start)
User --> (Use the application) : A small label

:Main Admin: ---> (Use the application) : This is\nyet another\nlabel

@enduml
```

```packetdiag
  colwidth = 32
  node_height = 72

  0-15: Source Port
  16-31: Destination Port
  32-63: Sequence Number
  64-95: Acknowledgment Number
  96-99: Data Offset
  100-105: Reserved
  106: URG [rotate = 270]
  107: ACK [rotate = 270]
  108: PSH [rotate = 270]
  109: RST [rotate = 270]
  110: SYN [rotate = 270]
  111: FIN [rotate = 270]
  112-127: Window
  128-143: Checksum
  144-159: Urgent Pointer
  160-191: (Options and Padding)
  192-223: data [colheight = 3]
```

```plantuml
@startuml

abstract class AbstractList
abstract AbstractCollection
interface List
interface Collection

List <|-- AbstractList
Collection <|-- AbstractCollection

Collection <|- List
AbstractCollection <|- AbstractList
AbstractList <|-- ArrayList

class ArrayList {
  Object[] elementData
  size()
}

enum TimeUnit {
  DAYS
  HOURS
  MINUTES
}

annotation SuppressWarnings

@enduml
```

```plantuml
@startuml
clock clk with period 1
binary "Enable" as EN

@0
EN is low

@5
EN is high

@10
EN is low
@enduml
```

```plantuml
@startuml
clock clk with period 1
binary "enable" as EN
concise "dataBus" as db

@0 as :start
@5 as :en_high 
@10 as :en_low


@:start
EN is low
db is "0x0000"

@:en_high
EN is high

@:en_low
EN is low

@:en_high-2
db is "0xf23a"

@:en_high+6
db is "0x0000"
@enduml
```

This is a rack diag schematic.
```rackdiag
// define height of rack
  16U;

  // define rack items
  1: UPS [2U];
  3: DB Server
  4: Web Server
  5: Web Server
  6: Web Server
  7: Load Balancer
  8: L3 Switch
```

```plantuml
@startuml
scale 5 as 150 pixels

clock clk with period 1
binary "enable" as en
binary "R/W" as rw
binary "data Valid" as dv
concise "dataBus" as db
concise "address bus" as addr

@6 as :write_beg
@10 as :write_end

@15 as :read_beg
@19 as :read_end


@0
en is low
db is "0x0"
addr is "0x03f"
rw is low
dv is 0

@:write_beg-3
 en is high
@:write_beg-2
 db is "0xDEADBEEF"
@:write_beg-1
dv is 1
@:write_beg
rw is high


@:write_end
rw is low
dv is low
@:write_end+1
rw is low
db is "0x0"
addr is "0x23"

@12
dv is high
@13 
db is "0xFFFF"

@20
en is low
dv is low
@21 
db is "0x0"

highlight :write_beg to :write_end #Gold:Write
highlight :read_beg to :read_end #lightBlue:Read

db@:write_beg-1 <-> @:write_end : setup time
db@:write_beg-1 -> addr@:write_end+1 : hold
@enduml
```



```plantuml
@startsalt
{+
{* File | Edit | Source | Refactor }
{/ General | Fullscreen | Behavior | Saving }
{
{ Open image in: | ^Smart Mode^ }
[X] Smooth images when zoomed
[X] Confirm image deletion
[ ] Show hidden images
}
[Close]
}
@endsalt


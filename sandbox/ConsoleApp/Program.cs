using System.Text.Json;
using System.Text.Json.Nodes;
using ZLinq;

var xs = new[] { 1, 2, 3 };

// xs.AsStructEnumerable().Select(x => x);`

var json = JsonNode.Parse("""
{
    "nesting": {
      "level1": {
        "description": "First level of nesting",
        "value": 100,
        "level2": {
          "description": "Second level of nesting",
          "flags": [true, false, true],
          "level3": {
            "description": "Third level of nesting",
            "coordinates": {
              "x": 10.5,
              "y": 20.75,
              "z": -5.0
            },
            "level4": {
              "description": "Fourth level of nesting",
              "metadata": {
                "created": "2025-02-15T14:30:00Z",
                "modified": null,
                "version": 2.1
              },
              "level5": {
                "description": "Fifth level of nesting",
                "settings": {
                  "enabled": true,
                  "threshold": 0.85,
                  "options": ["fast", "accurate", "balanced"],
                  "config": {
                    "timeout": 30000,
                    "retries": 3,
                    "deepSetting": {
                      "algorithm": "advanced",
                      "parameters": [1, 1, 2, 3, 5, 8, 13]
                    }
                  }
                }
              }
            }
          }
        }
      },
      "alternativePath": {
        "branchA": {
          "leaf1": "End of branch A path"
        },
        "branchB": {
          "section1": {
            "leaf2": "End of branch B section 1"
          },
          "section2": {
            "leaf3": "End of branch B section 2"
          }
        }
      }
    }
}
""");


var origin = json!["nesting"]!["level1"]!["level2"]!;

foreach (var item in origin.Descendants())
{
    if (item.Node == null)
    {
        Console.WriteLine(item.Name);
    }
    else
    {
        Console.WriteLine(item.Node.GetPath() + ":" + item.Name);
    }
}

// je.RootElement.ValueKind == System.Text.Json.JsonValueKind.Object


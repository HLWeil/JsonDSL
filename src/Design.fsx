{"assays": [
    {
        "filename": "MyAssay/isa.assay.xlsx",
        "characteristicCategories": [
            {
                "characteristicType": {
                    "annotationValue": "Sample type",
                    "termSource": "NFDI4PSO",
                    "termAccession": "http://purl.obolibrary.org/obo/NFDI4PSO_0000064",
                    "comments": [
                        {
                            "name": "ValueIndex",
                            "value": "1"
                        }
                    ]
                }
            },
            {
                "characteristicType": {
                    "annotationValue": "Biological replicate",
                    "termSource": "NFDI4PSO",
                    "termAccession": "http://purl.obolibrary.org/obo/NFDI4PSO_0000042",
                    "comments": [
                        {
                            "name": "ValueIndex",
                            "value": "2"
                        }
                    ]
                }
            }
        ]
    }
]
}


JNode =
    JValue
    JArray
    JObject

ObjectBuilder : object
    field : string -> 'A -> JNode
    //field : string -> JNode -> JNode

ArrayBuilder : array
    yield 'A
    yield JNode



object {
    field "filename" "MyAssay/isa.assay.xlsx"
    field "characteristicCategories" (
        array {
            object {
                field  "characteristicType" (
                    object {
                        field "annotationValue" "Sample type"
                        field "termSource" "NFDI4PSO"
                        field "termAccession" "http://purl.obolibrary.org/obo/NFDI4PSO_0000064"
                        field "comments" (
                            array {
                                object {
                                    field "name" "ValueIndex"
                                    field "value" 1
                                }                        
                            }
                        )

                    }
                
                )
            
            }
            object {
                field  "characteristicType" (
                    object {
                        field "annotationValue" "Biological replicate"
                        field "termSource" "NFDI4PSO"
                        field "termAccession" "http://purl.obolibrary.org/obo/NFDI4PSO_0000042"
                        field "comments" (
                            array {
                                object {
                                    field "name" "ValueIndex"
                                    field "value" 2
                                }                        
                            }
                        )
                    }                
                )            
            }
        }  
    )
}
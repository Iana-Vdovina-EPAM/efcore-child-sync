# efcore-child-sync

## Summary
This repository is an example of child entity syncronization differencies in two different Entity Framework Core versions: 2.2.0 and 3.1.5.

## Details
We have Entity - parent class and Child - child class. 
One Entity stores one Child.
Entity has properties: 
1. Id - unique identifier
2. Description - any string value
3. Child - full child entity
4. ChildCode - unique child identifier.

Child has properties:
1. ChildCode - unique identifier
2. Description - any string value

## Use case
1. Create entity1 (origin in the code) with child1 (ChildCode = "ChildCode-Original")
2. Create entity2 (changed in the code) with child2 (ChildCode = "ChildCode-Changed")
3. Save entity1 fully into the db
4. Set entity2.ChildCode = entity1.ChildCode
5. Add entity2 in the DbSet. Here Child property is automatically syncronized with ChildCode inside the EF core logic. 

EF Core 2.2 behavior
Entity2.ChildCode stores "ChildCode-Original" value and Entity2.Child = Entity1.Child.
Child and ChildCode are syncrinized based on *ChildCode* value.

EF Core 3.1 behavior
Entity2.ChildCode stores "ChildCode-Changed" and Entity2.Child != Entity1.Child.
Child and ChildCode are syncrinized based on *Child* value.

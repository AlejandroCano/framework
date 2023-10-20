using System.ComponentModel;
using static Npgsql.Replication.PgOutput.Messages.RelationMessage;
using System.Diagnostics.Metrics;
using Signum.Entities;

namespace Signum.DynamicQuery.Tokens;

public enum FilterFieldMessage
{
    FiltersHelp,
    [Description("A filter consists of a {0}, a comparison {1} and a constant {2}.")]
    AFilterConsistsOfA0AComparison1AndAConstant2,
    Field,
    Operator,
    Value,
    [Description("Field can be any field of the {0}, or any related entity.")]
    FieldCanBeAnyFieldOfThe0OrAnyRelatedEntity,
    [Description("Field can be any column of the query {0}, or any field of {1}.")]
    FieldCanBeAnyColumnOfTheQuery0OrAnyFieldOf1,
    [Description("AND / OR Groups")]
    AndOrGroups,
    [Description("Using [+ Add OR Group] you can group a few filters together so that only one condition needs to be satisfied. Inside an OR Group you can create a nested AND Group and so on. ")]
    UsingAddOrGroupYouCanGroupAFewFiltersTogether,
    [Description("Filter groups can also be used to combine filters that should be satisfied by the same element of a collection when using operator like Any or All in the prefix field.")]
    FilterGroupsCanAlsoBeUsedToCombineFiltersThatShouldBeSatisfiedByTheSameElement,
}

public enum FieldExpressionMessage
{
    LearnMoreAboutFieldExpressions,

    [Description("You can navigate database relationships by continuing the expression with more items.")]
    YouCanNavigateDatabaseRelationshipsByContinuingTheExpressionWithMoreItems,

    SimpleValues,
    [Description("A string (like \"Hello\") a number (like 3.14) or a boolean (true). Sometimes you will be able to continue the expression, like the {0} of a string or calculating the {1} or {2} of a number (for histograms).")]
    AStringLikeHelloANumberLike,

    Dates,
    [Description("{0} and {1}, you can extracts parts of the date by continuing the expression with {2} (return a number) or {3} (return a date)")]
    _0And1YouCanExtractsPartsOfTheDateByContinuingTheExpressionWith2ReturnANumberOr3ReturnADate,
    [Description("Entity Relationships")]
    EntityRelationships,
    EntityRelationshipsAllowYouToNavigateToOtherTablesToGetFields,
    [Description("in SQL")]
    InSql,

    Collections,
    [Description("Collection of entities or relationships.")]
    CollectionOfEntitiesOrRelationships,

    CollectionOperators,
    [Description("Multiplies the number of rows by all the elements in the collection. ({0} / {1} {2}). All the field expressions using the same {3} reuse the same {4}, to avoid this use {5} / {6}.")]
    MultipliesTheNumberOfRowsByAllTheElementsInTheCollection012,
    [Description("Allows to add filters that use conditions on the collection elemens (without multiplying the number of rows) ({0} {1}). To combine different conditons use {2} / {3} groups with a prefix (see below).")]
    AllowsToAddFiltersThatUseConditionsOnTheCollectionElemens,
    Aggregates,
    [Description("When allows to collapse many values in one value")]
    WhenGroupingAllowsToCollapseManyValuesInOneValue,
    [Description("Count Not Null")]
    CountNotNull,
    [Description("Count Distinct")]
    CountDistinct,
    [Description("Can only be used after another field.")]
    CanOnlyBeUsedAfterAnotherField,
    [Description("Finally, remember that you can {0} / {1} full field expression to other filters or columns by opening the drop-down-list and using {2} / {3}.")]
    FinallyRememberThatYouCan01FullFieldExpression,
}

public enum ColumnFieldMessage
{
    ColumnsHelp,
    ModifyingColumns,
    
    [Description("The default columns can be changed at will by {0} in a column header and then select {1}, {2} or {3}. You can also {4} the columns by dragging and dropping them to another position.")]
    TheDefaultColumnsCanBeChangedAtWillBy0InAColumnHeaderAndThenSelect1Or2Or3,
    
    [Description("rearrange")]
    Rearrange,
    [Description("right-clicking")]
    RightClicking,
    [Description("right-click")]
    RightClick,
    
    [Description("When inserting, the new column will be added before or after the selected column, depending where you {0}.")]
    WhenInsertingTheNewColumnWillBeAddedBeforeOrAfterTheSelectedColumn,
    
    [Description("Once editing a column, the following fields are available:")]
    OnceEditingAColumnTheFollowingFieldsAreAvailable,   
    
    [Description("You can select a field expression to point to any column of the query {0}, or any field of {1} or any related entity.")]
    YouCanSelectAFieldExpressionToPointToAnyColumnOfTheQuery0OrAnyFieldOf1OrAnyRelatedEntity,
    
    [Description("You can select a field expression to point to any field of the {0}, or any related entity.")]
    YouCanSelectAFieldExpressionToPointToAnyFieldOfThe0OrAnyRelatedEntity,
    
    [Description("The column header text is typically automatically set depending on the field expression, but can be customized by setting {0} manually.")]
    TheColumnHeaderTextIsTypicallyAutomaticallySetDependingOnTheFieldExpression,
    
    [Description("You can add one numeric value to the column header (like the total sum of the invoices), using a field expression ending in an aggregate (like {0},...). Note: The aggregation includes rows that may not be visible due to pagination!")]
    YouCanAddOneNumericValueToTheColumnHeaderLikeTheTotalSumOfTheInvoices,
    
    CombineValues,

    [Description("When a table has many repeated values in a column you can combine them vertically ({0}) either when the value is the same, or when is the same and belongs to the same main entity.")]
    WhenATableHasManyRepeatedValuesInAColumnYouCanCombineThemVertically,

    [Description("Grouping results by one (or more) column")]
    GroupingResultsByOneOrMoreColumn,
    
    [Description("You can group results by {0} in a column header and selecting {1}. All the columns will disapear except the selected one and an agregation column (typically {2}).")]
    YouCanGroupResultsBy0InAColumnHeaderAndSelecting1,
    
    [Description("Any new column should either be an aggregate or it will be considered a new {0}.")]
    AnyNewColumnShouldEitherBeAnAggregateOrItWillBeConsideredANew0,
    
    [Description("Once grouping you can filter normally or using aggregates in your fields ({0} {1}).")]
    OnceGroupingYouCanFilterNormallyOrUsingAggregatesInYourFields,
    
    [Description("Finally you can stop grouping by {0} in a column header and select {1}.")]
    FinallyYouCanStopGroupingBy0InAColumnHeaderAndSelect1,

    OrderingResults,
    
    [Description("You can order results by clicking in a column header, defualt ordering is Ascending and by clicking again it changes to Descending. You can order by more than one column if you keep {0} down when clicking on the columns header.")]
    YouCanOrderResultsByClickingInAColumnHeaderDefualtOrderingIsAscending,
    
    [Description("SummaryHeaderField")]
    SummaryHeaderField,
    
    [Description("Activate 'SummaryHeader' to add an aggregate for the whole query (like the sum of some numeric value) using and field expression.")]
    ActivateSummaryHeaderToAddAnAggregateForTheWholeQuery,
    
    [Description("Note: The aggregation includes rows that may not be visible due to pagination.")]
    NoteTheAggregationIncludesRowsThatMayNotBeVisibleDueToPagination,

}

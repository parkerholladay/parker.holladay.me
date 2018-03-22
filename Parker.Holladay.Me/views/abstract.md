[post]: posts/@Model.Slug
[image]: content/images/@Model.Image

<div class="abstract">

## [@Model.PageTitle][post]

[![@Model.PageTitle][image]][post]

<span class="abstract-date post-date">
@If.HasDate
@Model.Date
@EndIf
@IfNot.HasDate
Coming soon
@EndIf
</span>
@Each.Tags
<span class="abstract-tag post-tag">@Current</span>
@EndEach

</div>

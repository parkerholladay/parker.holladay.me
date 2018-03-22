[image]: ../content/images/@Model.Image

![@Model.PageTitle][image]

# @Model.PageTitle

<div class="post-date">
@If.HasDate
@Model.Date
@EndIf
@IfNot.HasDate
Coming soon
@EndIf
</div>
@Each.Tags
<div class="post-tag">@Current</div>
@EndEach

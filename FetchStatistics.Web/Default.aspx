<%-- 
This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
It is licensed under Microsoft Reciprocal License (Ms-RL).
--%>
<%@ Page Language="C#" CodeBehind="Default.aspx.cs" Inherits="FetchStatistics.Web.Default" EnableViewState="false" %>
<%@ Import Namespace="FetchStatistics.Web" %>
<%@ OutputCache Duration="600" VaryByParam="sort;rev;country" VaryByContentEncoding="gzip;deflate" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--<%=DateTime.Now%>-->
    <title>Latvijas slēpņotāju statistika</title>
    <style type="text/css">
        body
        {
            font-size: 12px;
            margin: 0;
            padding: 0;
            border: 0;
        }
        
        div#top-menu
        {
            height: 50px;
            padding: 0px;
            overflow: hidden;
        }
        
        span#title
        {
            display: block;
            font-weight: bold;
            font-family: Cambria, Times New Roman;
            font-size: 30px;
            margin: 5px 0px 0px 10px;
        }
        
        table#table-header
        {
            table-layout: fixed;
            border-collapse: collapse;
            position: absolute;
            width: 2630px;
            z-index: 100;
        }
        
        table#table-body
        {
            table-layout: fixed;
            border-collapse: collapse;
            width: 2630px;
        }

        table#table-left-header
        {
            table-layout: fixed;
            border-collapse: collapse;
            width: 130px;
            top: 50px;
            left: 0px;
            position: absolute;
            z-index: 150;
            background: red !important;
        }
                
        table#table-left-body
        {
            table-layout: fixed;
            border-collapse: collapse;
            width: 130px;
            position: absolute;
            top: 50px;
            background: white;
            z-index: 50;
        }
        
        tbody tr.alt
        {
        }
        
        th, td
        {
            overflow: hidden;
            border: 1px solid #c0c0c0;
        }
        
        th
        {
            background: #b7caed;
            height: 50px;
            font-weight: bold;
            background-repeat: no-repeat;
            background-position: right bottom;
            width: 100px;
        }
        
        th.sort-asc
        {
            background-color: #83abd5; 
            background-image: url(Styles/asc.png);
        }
        
        th.sort-desc
        {
            background-color: #83abd5; 
            background-image: url(Styles/desc.png);
        }
        
        th a
        {
            color: Navy;
            text-decoration: none;
        }
        
        td a
        {
            color: Black;
        }
        
        tr.selected
        {
            background: #fcf0c4;
            font-style: italic;
        }
        
        td
        {
            padding: 0px;
        }
        
        td span
        {
            padding: 2px 2px 2px 2px;
            overflow: visible;
            display: block;
        }
        
        select
        {
            font-size: 10px;
            width: 100px;
        }
    </style>
    <script type="text/javascript" src="Scripts/jquery.js"></script>
    <script type="text/javascript">

        var jWindow = $(window);
        var tableHeader;
        var tableLeftBody;
        var tableLeftHeader;

        jWindow.scroll(function () {
            if (!tableHeader) tableHeader = $("#table-header");
            if (!tableLeftBody) tableLeftBody = $("#table-left-body");
            if (!tableLeftHeader) tableLeftHeader = $("#table-left-header");

            var t = (Math.max(0, jWindow.scrollTop() - $("#top-menu").height()));
            var l = (jWindow.scrollLeft())

//            tableHeader
//              .stop()
//              .animate({ "marginTop": t }, "medium");
//            tableLeftHeader
//              .stop()
//              .animate({ "marginTop": t, "marginLeft": l }, "medium");
//            tableLeftBody
//              .stop()
//              .animate({ "marginLeft": l }, "medium");

            tableHeader.css("marginTop", t);
            tableLeftHeader.css("marginTop", t).css("marginLeft", l);
            tableLeftBody.css("marginLeft", l);
        });

        function attachRowClick() {
            $("tbody tr").click(function () {
                var index = $(this).index();
                $("#table-left-body tbody tr:eq(" + index + ")").toggleClass("selected");
                $("#table-body tbody tr:eq(" + index + ")").toggleClass("selected");
            });
        }

        $(document).ready(attachRowClick);

        function sortTable(link, isNumeric, firstDesc) {
            var th = $(link).closest("th");
            
            var reverse = th.hasClass("sort-asc");
            if (!reverse && !th.hasClass("sort-desc")) reverse = firstDesc;

            $("th").removeClass("sort-asc").removeClass("sort-desc");

            if (reverse)
                th.addClass("sort-desc");
            else
                th.addClass("sort-asc");

            var column = th.index();

            var rows1 = $("#table-body tbody tr");
            var rows2 = $("#table-left-body tbody tr");
            var values = new Array(rows1.length);
            for (var i = 0; i < rows1.length; i++) {
                var t = $(rows1[i].cells[column]).text();
                values[i] = [t, parseFloat(t) || -1, rows1[i], rows2[i]];
            }

            if (isNumeric)
                values.sort(reverse ? function (a, b) { return b[1] - a[1]; } : function (a, b) { return a[1] - b[1]; });
            else if (reverse)
                values.sort(function (a, b) { return (a[0] > b[0]) ? -1 : 1; });
            else
                values.sort();

            var lastTr1 = null;
            var lastTr2 = null;
            for (var i = 0; i < values.length; i++) {
                var tr1 = values[i][2];
                var tr2 = values[i][3];

                $(tr1.cells[0]).html(i + 1);
                $(tr2.cells[0]).html(i + 1);

                var currentIndex = tr1.sectionRowIndex;
                if (currentIndex == i) {
                    lastTr1 = $(tr1);
                    lastTr2 = $(tr2);
                    continue;
                }

                if (lastTr1 == null) {
                    $(tr1).remove().prependTo("#table-body tbody");
                    $(tr2).remove().prependTo("#table-left-body tbody");
                }
                else {
                    $(tr1).remove().insertAfter(lastTr1);
                    $(tr2).remove().insertAfter(lastTr2);
                }

                lastTr1 = tr1;
                lastTr2 = tr2;
            }

            attachRowClick();
        };

        function changeCountry(select) {
            var newValue = select.options[select.selectedIndex].innerHTML;
            var currentUrl = document.location.toString();
            currentUrl = currentUrl.replace(/[&?]country=[^&?]*$/, "");
            currentUrl = currentUrl + (currentUrl.indexOf("?") == -1 ? "?country=" : "&country=") + newValue;
            document.location = currentUrl;
        }
    </script>
    <script type="text/javascript">
        var _gaq = _gaq || [];
        _gaq.push(['_setAccount', 'UA-75304-4']);
        _gaq.push(['_trackPageview']);

        (function () {
            var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
            ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
            var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
        })();
    </script>
</head>
<body>
    <div id="top-menu">
        <span id="title">Latvijas slēpņotāju statistika</span>
    </div>
    <asp:Repeater runat="server" ID="Repeater">
        <HeaderTemplate>
    <table id="table-header">
        <thead>
            <tr>
                <th style="width: 30px;" >&nbsp;</th>
                <%= this.CreateHeader(o => o.UserName)%>
                <%= this.CreateHeader(o => o.MemberSince)%>
                <%= this.CreateHeader(o => o.FirstCacheDate)%>
                <%= this.CreateHeader(o => o.LastVisit)%>
                <%= this.CreateHeader(o => o.Updated)%>
                <%= this.CreateHeader(o => o.UpdatedBy)%>
                <%= this.CreateHeader(o => o.CacheFinds)%>
                <%= this.CreateHeader(o => o.DistinctCacheFinds)%>
                <%= this.CreateHeader(o => o.CacheFindsInCountry)%>
                <%= this.CreateHeader(o => o.CountryCount)%>
                <%= this.CreateHeader(o => o.CachesHidden)%>
                <%= this.CreateHeader(o => o.TrackablesLogged)%>
                <%= this.CreateHeader(o => o.FindRate)%>
                <%= this.CreateHeader(o => o.CurrentYearFindRate)%>
                <%= this.CreateHeader(o => o.LongestStreak)%>
                <%= this.CreateHeader(o => o.LongestSlump)%>
                <%= this.CreateHeader(o => o.BestDay)%>
                <%= this.CreateHeader(o => o.BestMonth)%>
                <%= this.CreateHeader(o => o.BestYear)%>
                <%= this.CreateHeader(o => o.DaysInYearCached)%>
                <%= this.CreateHeader(o => o.AverageDifficulty)%>
                <%= this.CreateHeader(o => o.AverageTerrain)%>
                <%= this.CreateHeader(o => o.DifficultyTerrainCombinations)%>
                <%= this.CreateHeader(o => o.Difficulty5Caches)%>
                <%= this.CreateHeader(o => o.Terrain5Caches)%>
            </tr>
        </thead>
    </table>
    <table id="table-body">
        <thead>
            <tr>
                <th style="width: 30px;" >&nbsp;</th>
                <%= this.CreateHeader(o => o.UserName)%>
                <%= this.CreateHeader(o => o.MemberSince)%>
                <%= this.CreateHeader(o => o.FirstCacheDate)%>
                <%= this.CreateHeader(o => o.LastVisit)%>
                <%= this.CreateHeader(o => o.Updated)%>
                <%= this.CreateHeader(o => o.UpdatedBy)%>
                <%= this.CreateHeader(o => o.CacheFinds)%>
                <%= this.CreateHeader(o => o.DistinctCacheFinds)%>
                <%= this.CreateHeader(o => o.CacheFindsInCountry)%>
                <%= this.CreateHeader(o => o.CountryCount)%>
                <%= this.CreateHeader(o => o.CachesHidden)%>
                <%= this.CreateHeader(o => o.TrackablesLogged)%>
                <%= this.CreateHeader(o => o.FindRate)%>
                <%= this.CreateHeader(o => o.CurrentYearFindRate)%>
                <%= this.CreateHeader(o => o.LongestStreak)%>
                <%= this.CreateHeader(o => o.LongestSlump)%>
                <%= this.CreateHeader(o => o.BestDay)%>
                <%= this.CreateHeader(o => o.BestMonth)%>
                <%= this.CreateHeader(o => o.BestYear)%>
                <%= this.CreateHeader(o => o.DaysInYearCached)%>
                <%= this.CreateHeader(o => o.AverageDifficulty)%>
                <%= this.CreateHeader(o => o.AverageTerrain)%>
                <%= this.CreateHeader(o => o.DifficultyTerrainCombinations)%>
                <%= this.CreateHeader(o => o.Difficulty5Caches)%>
                <%= this.CreateHeader(o => o.Terrain5Caches)%>
            </tr>
        </thead>
        <tbody>
        </HeaderTemplate>
        <ItemTemplate>
        <tr>
            <td><span><%#Container.ItemIndex + 1%></span></td>
            <td><a href="http://www.geocaching.com/profile/?guid=<%#Container.Value(o => o.UserId)%>"><%#Container.Value(o => o.UserName)%></a></td>
            <td><%#Container.Value(o => o.MemberSince)%></td>
            <td><%#Container.Value(o => o.FirstCacheDate)%></td>
            <td><%#Container.Value(o => o.LastVisit)%></td>
            <td><%#Container.Value(o => o.Updated)%></td>
            <td><%#Container.Value(o => o.UpdatedBy)%></td>
            <td><%#Container.Value(o => o.CacheFinds)%></td>
            <td><%#Container.Value(o => o.DistinctCacheFinds)%></td>
            <td><%#Container.Value(o => o.CacheFindsInCountry)%></td>
            <td><%#Container.Value(o => o.CountryCount)%></td>
            <td><%#Container.Value(o => o.CachesHidden)%></td>
            <td><%#Container.Value(o => o.TrackablesLogged)%></td>
            <td><%#Container.Value(o => o.FindRate)%></td>
            <td><%#Container.Value(o => o.CurrentYearFindRate)%></td>
            <td><%#Container.Value(o => o.LongestStreak)%></td>
            <td><%#Container.Value(o => o.LongestSlump)%></td>
            <td><%#Container.Value(o => o.BestDay)%></td>
            <td><%#Container.Value(o => o.BestMonth)%></td>
            <td><%#Container.Value(o => o.BestYear)%></td>
            <td><%#Container.Value(o => o.DaysInYearCached)%></td>
            <td><%#Container.Value(o => o.AverageDifficulty)%></td>
            <td><%#Container.Value(o => o.AverageTerrain)%></td>
            <td><%#Container.Value(o => o.DifficultyTerrainCombinations)%></td>
            <td><%#Container.Value(o => o.Difficulty5Caches)%></td>
            <td><%#Container.Value(o => o.Terrain5Caches)%></td>
        </tr>
        </ItemTemplate>
        <FooterTemplate>
        </tbody>
    </table>
        </FooterTemplate>
    </asp:Repeater>

    <asp:Repeater runat="server" ID="RepeaterLeft">
        <HeaderTemplate>
    <table id="table-left-header">
        <thead>
            <tr>
                <th style="width: 30px;" >&nbsp;</th>
                <%= this.CreateHeader(o => o.UserName)%>
            </tr>
        </thead>
    </table>
    <table id="table-left-body">
        <thead>
            <tr>
                <th style="width: 30px;" >&nbsp;</th>
                <%= this.CreateHeader(o => o.UserName)%>
            </tr>
        </thead>
        <tbody>
        </HeaderTemplate>
        <ItemTemplate>
        <tr>
            <td><span><%#Container.ItemIndex + 1%></span></td>
            <td><a href="http://www.geocaching.com/profile/?guid=<%#Container.Value(o => o.UserId)%>"><%#Container.Value(o => o.UserName)%></a></td>
        </tr>
        </ItemTemplate>
        <FooterTemplate>
        </tbody>
    </table>
        </FooterTemplate>
    </asp:Repeater>
</body>
</html>

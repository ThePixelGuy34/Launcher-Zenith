
var i18nMessages = {};
var defaultLanguage = "en";

// We should probably move this to it's own file (and shared across all pages with a web-view)
var config = {
    supportedLocales: ["en", "fr", "de", "es", "es-MX", "es-ES", "ko", "ja", "zh-CN", "it", "ru", "ar", "pt-BR", "pl", "tr"]
};

function getQueryParam(paramName)
{
    paramName = paramName.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    let regex = new RegExp("[\\?&]" + paramName + "=([^&#]*)"),
        results = regex.exec(window.location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

var loadI18N = function(lang, callback)
{
    if (typeof(lang) === 'function') {
        // Clean up function parameters.
        callback = lang;
        lang = null;
    }

    callback = callback || function (){};
    let doLoad = true;
    for (let prop in i18nMessages)
    {
        if (i18nMessages.hasOwnProperty(prop))
        {
            doLoad = false;
        }
    }
    
    if (doLoad) {
        if (!lang) {
            lang = getQueryParam("lang");
        }
        console.log("Requested lang: " + lang);
        // based on the languages we support, use the language library to do
        // a fallback on what to use (e.g. if don't support 'es-MX' but we support
        // 'es' and we are requested 'es-ES', then this will return 'es')
        // the function returns an array of strings
        let langArr = preferredLanguages(lang, config.supportedLocales);
        console.log("preferredLanguages: " + langArr);
        // Guarantee an entry by always pushing our default
        langArr.push(defaultLanguage);
        // Select first preferred that we support
        lang = langArr[0];
        // Load from file
        let fileName = "./i18_" + lang + ".json";
        console.log("Messages loading from file " + fileName);
        
        // If we're running locally, load the JSON file from the local filestorage
        $.getJSON(fileName,
            function(json) {
                i18nMessages = json;
                callback();
        })
        .fail(function( jqxhr, textStatus, error ) {
            console.log( "error: " + error );
            if (lang !== defaultLanguage) {
                console.log("Falling back to default language: " + defaultLanguage);
                loadI18N(defaultLanguage, callback);
            }
        });
    }
};

// TODO: Replace this with a proper way of loading i18 messages
function getI18Message(messageCode) {
    let message = i18nMessages[messageCode] === undefined ? messageCode : i18nMessages[messageCode];

    return message;
}

function updateLocStringsOnAnnotatedElements() {
    // Find all tags with the attribute "data-loc-code" and replace their contents
    // with a localized one
    let language = getQueryParam('lang');
    $("html").addClass(language);
            
    $("[data-loc-code]").each(function() {
        updateLocStringOnElement(this);
    });
}

function updateLocStringOnElement(element) {
    const locVal = getI18Message($(element).attr("data-loc-code"));

    if (locVal) {
        $(element).html(locVal);
        console.log(locVal); 
    }
}

$(document).ready(function () {
    console.log("DocumentReady()");
    
    // Disable right click context menu
    window.addEventListener("contextmenu",
        function (e) {
            e.preventDefault()
            return false;
        });
    
    var postJsBridgeInit = function () {
        console.log("postJsBridgeInit()");
        if (window.ue && window.ue.installer) {
            loadI18N(function() {
                const timePeriod = 30000;
                const plainNumberRegex = /^\d+\.?\d*$/;

                let data = [];
                let inrangedrawfileoperations = [];
                let d3formatMemory = d3.format(".3s");
                
                const formatSpeed = (d, suffix) => {
                    const preformattedMemory = d3formatMemory(d) + "B";
                    return (suffix) ? 
                        preformattedMemory + suffix : 
                        preformattedMemory;
                };

                const formatSpeedPerSecond = (d) => {
                  return formatSpeed(d, "/s");
                }

                updateLocStringsOnAnnotatedElements();
                
                function createFileOperationsChart(chart) {
                    let svg = d3.select("#" + chart.id + "-chart").append("svg")
                        .style("id", chart.id)
                        .style("width", chart.width)
                        .style("height", chart.height);

                    let margin = chart.margin;
                    let useablewidth = chart.width - margin.left - margin.right;
                    let useableheight = chart.height - margin.top - margin.bottom;
                    
                    let g = svg.append("g")
                        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

                    let y = d3.scaleLinear()
                        .domain([0, chart.view.width])
                        .range([0, useableheight]);
                        
                    let x = d3.scaleLinear()
                        .domain([0, chart.view.linesprefocus + chart.view.linespostfocus])
                        .range([0, useablewidth]);
                        
                    chart.x = x;
                    chart.y = y;
                    chart.useableheight = useableheight;
                    chart.useablewidth = useablewidth;
                }

                function updateDownloadHealthInfo(d3, args) {
                    // Get the enum value of the health so we can generate the loc attributes
                    const healthEnum = args.downloadhealth && args.downloadhealth.toLowerCase();

                    //If we have a health enum, set the appropriate messages and styles
                    if (healthEnum) {
                        const healthStatusElem = $("#health-downloadbytespeed-chart");
                        const healthInfoElem = $("#health-downloadbytespeed-info");
                        const downloadedSeparator = getI18Message("messages.com.epicgames.launcher.installer.downloadSize.downloadedCurrentOfTotalSeparator");
                        d3.select("#size-downloadbytespeed-chart").text(formatSpeed(args.totaldownloaded) + " " + downloadedSeparator + " " + formatSpeed(args.totaldownloadrequired));

                        // set classes on health status to match styling of status
                        healthStatusElem.attr("class", "installerText downloadHealthText " + (healthEnum || 'none'));

                        // set data-loc-code for health value then update i18n strings
                        healthStatusElem.attr("data-loc-code", "messages.com.epicgames.launcher.installer.downloadHealth.status." + healthEnum);
                        healthInfoElem.attr("data-loc-code", "messages.com.epicgames.launcher.installer.downloadHealth.info." + healthEnum);
                        updateLocStringOnElement(healthStatusElem);
                        updateLocStringOnElement(healthInfoElem);
                    } else {
                        // We didn't get a health enum value. Hide status so we don't show garbage
                        const healthStatusElem = $("#health-downloadbytespeed-chart");
                        healthStatusElem.attr("class", "installerText downloadHealthText none");
                    }
                }

                function updateFileOperationsChart(chart) {
                    let margin = chart.margin;
                    let root = d3.select("#" + chart.id + "-chart");
                    let g = root.select("g");
                    
                    let rangefileoperationsoffset = 0;
                    let first = inrangedrawfileoperations[inrangedrawfileoperations.length - 1];
                    
                    if (first) {
                        rangefileoperationsoffset = first.left;
                    }
                    
                    g.attr("transform", "translate(" + ((margin.left - chart.x(rangefileoperationsoffset + 1)) + chart.useablewidth) + "," + margin.top + ")");
                    
                    let rects = g.selectAll("rect")
                        .data(inrangedrawfileoperations, (d) => { return d.id; });

                    rects
                        .attr("height", (d) => { return chart.y(d.width) + 1; })
                        .style("fill", (d) => { return chart.colors[d.state]; });
                        
                    rects.enter().append("rect")
                        .classed("bar", true)
                        .classed("visible", true)
                        .style("fill", (d) => { return chart.colors[d.state]; })
                        .style("opacity", 100)
                        .attr("_top", (d) => { return d.top; })
                        .attr("_left", (d) => { return d.left; })
                        .attr("_width", (d) => { return d.width; })
                        .attr("y", (d) => { return chart.y(d.top) - 1; })
                        .attr("width", (d) => { return chart.x(1) + 1; })
                        .attr("x", (d) => { return chart.x(d.left) - 1; })
                        .attr("height", (d) => { return chart.y(d.width) + 1; });
                            
                    rects.exit().filter(".visible")
                        .classed("visible", false)
                        .remove();
                }

                function createSpeedChart(chart) {

                    let svg = d3.select("#" + chart.id + "-chart").append("svg")
                        .style("id", chart.id)
                        .style("width", chart.width)
                        .style("height", chart.height);

                    let defs = svg.append("defs");

                    let gradient = defs.append("linearGradient")
                       .attr("id", "areaGradient-" + chart.id)
                       .attr("x1", "100%")
                       .attr("x2", "0%")
                       .attr("y1", "100%")
                       .attr("y2", "95%");

                    gradient.append("stop")
                       .attr('class', 'start')
                       .attr("offset", "0%")
                       .attr("stop-color", chart.colors[1])
                       .attr("stop-opacity", 0.5);

                    gradient.append("stop")
                       .attr('class', 'end')
                       .attr("offset", "90%")
                       .attr("stop-color", chart.colors[1])
                       .attr("stop-opacity", 0);

                    let    margin = chart.margin,
                    useablewidth = chart.width - margin.left - margin.right,
                    useableheight = chart.height - margin.top - margin.bottom,
                    g = svg.append("g")
                        .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

                    // set the ranges
                    let x = d3.scaleTime().range([0, useablewidth]);
                    let y = d3.scaleLinear().range([useableheight, 0]);
                    chart.x = x;
                    chart.y = y;
                    chart.useableheight = useableheight;
                    chart.useablewidth = useablewidth;
                    
                    // define the area
                    let area = d3.area()
                        .x((d) => { return x(d.date); })
                        .y0(useableheight + 1)
                        .y1((d) => { return y(d[chart.id]); });
                    chart.area = area;
                    
                    // define the line
                    let valueline = d3.line()
                        .x((d) => { return x(d.date); })
                        .y((d) => { return y(d[chart.id]); });
                    chart.valueline = valueline;
                        
                    x.domain(d3.extent(data, (d) => { return d.date; }));
                    y.domain([0, d3.max(data, (d) => { return d[chart.id]; })]);

                    let yaxis = d3.axisLeft(y)
                        .tickFormat(formatSpeedPerSecond)
                        .tickSize(-useablewidth);
                    chart.yaxis = yaxis;

                    
                    g.append("path")
                        .classed("line", true)
                        .attr("id", chart.id + "-line")
                        .style("stroke", chart.colors[0])
                        .attr('d', valueline(data));

                    g.append("path")
                        .classed("area", true)
                        .attr("id", chart.id + "-area")
                        .style("fill", "url(#areaGradient-" + chart.id + ")")
                        .attr('d', area(data));
                        
                    g.append("line")
                        .classed("speedline", true)
                        .attr("id", chart.id + "-speedline")
                        .style("stroke", chart.colors[0])
                        .attr("y1", chart.y.range()[0])
                        .attr("y2", chart.y.range()[1]);
                    
                    g.append("text")
                        .classed("speedlinetext", true)
                        .attr("id", chart.id + "-speedlinetext")
                        .attr("y", chart.y.range()[1] + 10)
                        .attr("text-anchor", "start");
                            
                    let focus = g.append("g")
                        .classed("focus", true)
                        .attr("id", chart.id + "-focus")
                        .style("stroke", chart.colors[0])
                        .style("display", "none");

                    focus.append("circle")
                        .attr("r", 4.5);

                    focus.append("text")
                        .attr("x", 9)
                        .attr("dy", ".35em");
                    
                    //g.append("rect")
                    //    .classed("overlay", true)
                    //    .attr("id", chart.id + "-overlay")
                    //    .attr("width", useablewidth)
                    //    .attr("height", useableheight)
                    //    .on("mouseover", mouseover)
                    //    .on("mouseout", mouseout)
                    //    .on("mousemove", mousemove);
                }

                function updateSpeedChart(chart, bUpdateYAxis) {
                    let extentDate = new Date(data[0].date);
                    let missingTime = timePeriod - new Date(data[0].date - data[data.length - 1].date);
                    
                    extentDate.setTime(extentDate.getTime() + missingTime);
                    
                    chart.x.domain([data[data.length - 1].date, extentDate]);
                    chart.y.domain([0, d3.max(data, (d) => { return d[chart.id]; })]);

                    d3.select("#" + chart.id + "-line").attr('d', chart.valueline(data));
                    d3.select("#" + chart.id + "-area").attr('d', chart.area(data));
                    
                    let speedlinex = chart.x(data[0].date);
                    d3.select("#" + chart.id + "-speedline")
                        .attr("x1", speedlinex)
                        .attr("x2", speedlinex);
                    
                    let currentspeed = data[0][chart.id];
                    d3.select("#" + chart.id + "-speedlinetext")
                        .attr("x", speedlinex + 5)
                        .text(formatSpeedPerSecond(currentspeed));
                }

                function mouseover() {
                    for (index = 0; index < speedCharts.length; ++index) {
                        let chart = speedCharts[index];
                        let focus = d3.select("#" + chart.id + "-focus");
                        focus.style("display", null);
                    }
                }

                function mouseout() {
                    for (index = 0; index < speedCharts.length; ++index) {
                        let chart = speedCharts[index];
                        let focus = d3.select("#" + chart.id + "-focus");
                        focus.style("display", "none");
                    }
                }

                function mousemove() {
                    let bisectDate = d3.bisector(function(d) { return d.date; }).left;

                    for (index = 0; index < speedCharts.length; ++index) {
                        let chart = speedCharts[index];
                        let x0 = chart.x.invert(d3.mouse(this)[0]);
                        let bisectIndex = bisectDate(data, x0, 1);
                        
                        let d0 = data[bisectIndex - 1];
                        let d = d0;
                        if (bisectIndex < data.length - 1) {
                            let d1 = data[bisectIndex];
                            d = x0 - d0.date > d1.date - x0 ? d1 : d0;
                        }
                        
                        if (d) {
                            let focus = d3.select("#" + chart.id + "-focus");
                            focus.attr("transform", "translate(" + chart.x(d.date) + "," + chart.y(d[chart.id]) + ")");
                            focus.select("text").text(formatSpeedPerSecond(d[chart.id]));
                        }
                    }
                }

                let speedChartMargin = {top: 5, right: 80, bottom: 5, left: 0};
                let speedCharts = [
                    { id:'downloadbytespeed', label:'Download Speed', colors: [ "#a6c6d6", "#90b1cb" ], width:474, height:60, margin: speedChartMargin },
                    { id:'diskreadbytespeed', label:'Disk Read Speed', colors: [ "#b1a6d6", "#a390cb" ], width:474, height:60, margin: speedChartMargin },
                    { id:'diskwritebytespeed', label:'Disk Write Speed', colors: [ "#afd6a6", "#95cb90" ], width:474, height:60, margin: speedChartMargin }
                ];

                for (i = 0; i < speedCharts.length; ++i) {
                    createSpeedChart(speedCharts[i]);
                }

                var stateColors =[
                    "c3c2c2",  // Unknown - Grey
                    
                    "a390cb",  // PendingLocalChunkDbData - Aqua 
                    "b1a6d6",  // RetrievingLocalChunkDbData - Light Aqua
                    
                    "a390cb",  // PendingLocalInstallData 
                    "b1a6d6",  // RetrievingLocalInstallData - Light
                    
                    "90b1cb",  // PendingRemoteCloudData - Purple
                    "a6c6d6",  // RetrievingRemoteCloudData - Light Purple
                    
                    "a390cb",  // PendingLocalDataStore
                    "b1a6d6",  // RetrievingLocalDataStore
                    
                    "c3c2c2",  // DataInMemoryStore
                    
                    "c8e2bf",  // Staged - Light Green 3
                    "c8e2bf",  // Installed - Green 1
                    
                    "afd6a6",  // Verifying - Orange
                    "rgb(214, 39, 40)", // VerifiedFail - Red
                    "95cb90"   // VerifiedSuccess - Green
                ];
                var fileOperationsChart = {
                    id:'fileoperation', 
                    colors: stateColors, 
                    width: 434, 
                    height: 70, 
                    margin: {top: 10, right: 0, bottom: 0, left: 0},
                    view: {
                        linesprefocus: 1,
                        linespostfocus: 50,
                        minlinethreshold: 1,
                        maxlinethreshold: 1,
                        width: 60,
                        bytesperunit: 1024 * 128,
                        minbarsize: 6,
                        maxbarsize: 60
                    }
                };
                createFileOperationsChart(fileOperationsChart);

                ue.installer.setfileoperationview(fileOperationsChart.view);

                const updateHandle = ue.installer.subscribe('OnUpdate', (args) => {
                    inrangedrawfileoperations = args.inrangedrawfileoperations;
                    args.inrangedrawfileoperations = null;
                    
                    let parseTime = d3.utcParse("%Y-%m-%dT%H:%M:%S.%L%Z");
                    args.date = parseTime(args.samplingdatetime);
                    data.unshift(args);

                    let lastDate = new Date(data[0].date.getTime());
                    lastDate.setTime(lastDate.getTime() - timePeriod);
                    for (i = data.length - 1; i > 0; --i) { 
                        if (data[i].date < lastDate) {
                            data.splice(i, 1);
                        } else {
                            break;
                        }
                    }
                    
                    updateDownloadHealthInfo(d3, args);
                    
                    for (let speedIndex = 0; speedIndex < speedCharts.length; ++speedIndex) { 
                        updateSpeedChart(speedCharts[speedIndex]);
                    }
                    
                    updateFileOperationsChart(fileOperationsChart);
                });
                  
                const installerChangedHandle = ue.installer.subscribe('OnInstallerChanged', (args) => {
                    inrangedrawfileoperations = [];
                    data = [];

                    for (let speedIndex = 0; speedIndex < speedCharts.length; ++speedIndex) { 
                        updateSpeedChart(speedCharts[speedIndex]);
                    }
                    
                    updateFileOperationsChart(fileOperationsChart);
                });

            });
        }
        else {
            console.log("Could not locate window.ue.installer object, trying again in 50ms.");
            setTimeout(postJsBridgeInit, 50);
        }
    };
    postJsBridgeInit();
});
import { StreamStatisticsResult } from '@/lib/iptvApi'
import React from 'react'
import { Line, LineChart } from 'recharts'

const StreamingStatusGraph = (props: StreamingStatusGraphProps) => {
  const [chartOptions, setChartOptions] = React.useState({})
  const documentStyle = getComputedStyle(document.documentElement)
  const [dataSource, setDataSource] = React.useState<StreamStatisticsResult[]>(
    [] as StreamStatisticsResult[],
  )
  // const [dataSet, setDataSet] = React.useState<GraphData>({} as GraphData);

  React.useEffect(() => {
    if (chartOptions === undefined || chartOptions === null) {
      const textColor = documentStyle.getPropertyValue('--text-color')
      const textColorSecondary = documentStyle.getPropertyValue(
        '--text-color-secondary',
      )
      const surfaceBorder = documentStyle.getPropertyValue('--surface-border')

      const options = {
        aspectRatio: 0.6,
        maintainAspectRatio: true,
        plugins: {
          legend: {
            labels: {
              color: textColor,
            },
          },
        },
        responsive: true,
        scaleOverride: true,
        scales: {
          x: {
            grid: {
              color: surfaceBorder,
            },
            ticks: {
              color: textColorSecondary,
            },
          },
          y: {
            grid: {
              color: surfaceBorder,
            },
            ticks: {
              color: textColorSecondary,
            },
          },
        },
        scaleSteps: 5,
        scaleStepWidth: 10,
      }

      setChartOptions(options)
    }
  }, [chartOptions, documentStyle])

  React.useEffect(() => {
    setDataSource([...dataSource, ...props.dataSource].slice(-6))
  }, [props.dataSource])

  const dataSet = React.useMemo((): GraphDataArray[] => {
    const toReturn = [] as GraphDataArray[]

    dataSource.forEach((item) => {
      if (item.m3UStreamName === undefined || item.m3UStreamName === '') {
        return
      }

      const newDataSet = {} as GraphDataArray

      if (
        Object.keys(newDataSet).length === 0 ||
        !newDataSet[item.m3UStreamName]
      ) {
        newDataSet[item.m3UStreamName] = {
          data: [item.inputBitsPerSecond ?? 0],
          name: item.m3UStreamName,
        }
      } else {
        newDataSet[item.m3UStreamName].data.push(item.inputBitsPerSecond ?? 0)
        newDataSet[item.m3UStreamName].name = item.m3UStreamName
      }

      toReturn.push(newDataSet)
      //   if (newDataSet.graphData.length === 0 || newDataSet.graphData.findIndex((x: GraphData) => x.name === item.m3UStreamName) === -1) {
      //     newDataSet.datasets.push({
      //       borderColor: documentStyle.getPropertyValue('--blue-500'),
      //       data: [item.inputBitsPerSecond ?? 0],
      //       fill: false,
      //       label: item.m3UStreamName ?? '',
      //       tension: 0.4
      //     });
      //     newDataSet.labels.push('');
      //   } else {
      //     const index = newDataSet.datasets.findIndex((x) => x.label === item.m3UStreamName);
      //     newDataSet.datasets[index].data.push(item.inputBitsPerSecond ?? 0);

      //     const dataSetLength = newDataSet.datasets[index]?.data.length || 0;
      //     const numberOfLabels = Math.min(dataSetLength, 30);

      //     const historyLabels: string[] = [];

      //     for (let i = -30; i <= 0; i++) {
      //       if (i >= -numberOfLabels) {
      //         historyLabels.push(i.toString());
      //       } else {
      //         historyLabels.push('');
      //       }
      //     }

      //     newDataSet.labels = historyLabels;
      //   }

      //   newDataSet.datasets = newDataSet.datasets.slice(-30);
      //   newDataSet.labels = newDataSet.labels.slice(-30);
    })

    return toReturn
  }, [dataSource])

  const getXValue = (data: GraphDataArray, name: string) => {
    if (data[name] === undefined || data[name] === null) return 0

    // console.debug(data[name].data);
    return data[name].data
  }

  return (
    <div className={`flex ` + props.className} style={props.style}>
      {/* <Chart data={props.isLoading === true ? undefined : dataSet} options={chartOptions} type="line" /> */}
      <LineChart data={dataSet} height={400} width={400}>
        <Line
          dataKey={(e) => getXValue(e, 'Brown Bunnies')}
          label="Brown Bunnies"
          stroke="#8884d8"
          type="monotone"
        />
      </LineChart>
    </div>
  )
}

StreamingStatusGraph.displayName = 'StreamingStatusGraph'

type StreamingStatusGraphProps = {
  readonly className?: string
  readonly dataSource: StreamStatisticsResult[]
  readonly style?: React.CSSProperties
}

export default React.memo(StreamingStatusGraph)

type GraphDataArray = {
  [key: string]: GraphData
}

type GraphData = {
  data: number[]
  name: string
}

// export type GraphData = {
//   datasets: Array<{
//     borderColor: string;
//     data: number[];
//     fill: boolean;
//     label: string;
//     tension: number;
//   }>;
//   labels: string[];
// }

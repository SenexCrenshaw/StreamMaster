/* eslint-disable @typescript-eslint/no-unused-vars */
import React from "react";
import { Chart } from "primereact/chart";
import { type StreamStatisticsResult } from "../store/iptvApi";
import { LineChart, Line } from 'recharts';

const StreamingStatusGraph = (props: StreamingStatusGraphProps) => {
  const [chartOptions, setChartOptions] = React.useState({});
  const documentStyle = getComputedStyle(document.documentElement);
  const [dataSource, setDataSource] = React.useState<StreamStatisticsResult[]>([] as StreamStatisticsResult[]);
  // const [dataSet, setDataSet] = React.useState<GraphData>({} as GraphData);

  React.useEffect(() => {
    if (chartOptions === undefined || chartOptions === null) {
      const textColor = documentStyle.getPropertyValue('--text-color');
      const textColorSecondary = documentStyle.getPropertyValue('--text-color-secondary');
      const surfaceBorder = documentStyle.getPropertyValue('--surface-border');
      const data = [{ name: 'Page A', uv: 400, pv: 2400, amt: 2400 }, ...];

      const options = {
        aspectRatio: 0.6,
        maintainAspectRatio: true,
        plugins: {
          legend: {
            labels: {
              color: textColor
            }
          }
        },
        responsive: true,
        scaleOverride: true,
        scales: {
          x: {
            grid: {
              color: surfaceBorder
            },
            ticks: {
              color: textColorSecondary
            }
          },
          y: {
            grid: {
              color: surfaceBorder
            },
            ticks: {
              color: textColorSecondary
            }
          }
        },
        scaleSteps: 5,
        scaleStepWidth: 10
      };

      setChartOptions(options);
    }
  }, [chartOptions, documentStyle]);


  React.useEffect(() => {
    setDataSource([...dataSource, ...props.dataSource].slice(-30));
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [props.dataSource]);

  const dataSet = React.useMemo((): GraphData => {
    const newDataSet = {} as GraphData;

    dataSource.forEach((item) => {
      if (!newDataSet || newDataSet.length === 0 || newDataSet.findIndex((x) => x.name === item.m3UStreamName) === -1) {
        newDataSet.datasets.push({
          borderColor: documentStyle.getPropertyValue('--blue-500'),
          data: [item.inputBitsPerSecond ?? 0],
          fill: false,
          label: item.m3UStreamName ?? '',
          tension: 0.4
        });
        newDataSet.labels.push('');
      } else {
        const index = newDataSet.datasets.findIndex((x) => x.label === item.m3UStreamName);
        newDataSet.datasets[index].data.push(item.inputBitsPerSecond ?? 0);

        const dataSetLength = newDataSet.datasets[index]?.data.length || 0;
        const numberOfLabels = Math.min(dataSetLength, 30);

        const historyLabels: string[] = [];

        for (let i = -30; i <= 0; i++) {
          if (i >= -numberOfLabels) {
            historyLabels.push(i.toString());
          } else {
            historyLabels.push('');
          }
        }

        newDataSet.labels = historyLabels;
      }

      newDataSet.datasets = newDataSet.datasets.slice(-30);
      newDataSet.labels = newDataSet.labels.slice(-30);
    });

    return newDataSet;
  }, [dataSource, documentStyle]);

  return (
    <div className={`flex ` + props.className} style={props.style}>
      {/* <Chart data={props.isLoading === true ? undefined : dataSet} options={chartOptions} type="line" /> */}
      <LineChart width={400} height={400} data={dataSource}>
        <Line type="monotone" dataKey="uv" stroke="#8884d8" />
      </LineChart>
    </div>
  )
}

StreamingStatusGraph.displayName = 'StreamingStatusGraph';
StreamingStatusGraph.defaultProps = {

};

type StreamingStatusGraphProps = {
  className?: string;
  dataSource: StreamStatisticsResult[];
  isLoading: boolean;
  style?: React.CSSProperties;
};

export default React.memo(StreamingStatusGraph);

export type GraphData = {
  name: string;
  amt: number;
};

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

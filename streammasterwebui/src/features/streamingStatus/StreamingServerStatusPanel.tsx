import React from 'react';
import { type StreamStatisticsResult } from '../../store/iptvApi';
import { formatJSONDateString, getTopToolOptions } from '../../common/common';
import StreamMasterSetting from '../../store/signlar/StreamMasterSetting';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import DataSelector from '../dataSelector/DataSelector';
import { Button } from 'primereact/button';
import { Toast } from 'primereact/toast';
import * as Hub from "../../store/signlar_functions";

export const StreamingServerStatusPanel = (props: StreamingServerStatusPanelProps) => {
  const setting = StreamMasterSetting();
  const toast = React.useRef<Toast>(null);

  const onFailStream = React.useCallback(async (rowData: StreamStatisticsResult) => {
    if (!rowData.streamUrl || rowData.streamUrl === undefined || rowData.streamUrl === '') {
      return;
    }

    await Hub.SimulateStreamFailure(rowData.streamUrl)
      .then(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Next Stream`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });
        }
      }).catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Next Stream Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error'
          });
        }
      });

  }, []);

  const imageBodyTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return (
      <div className="flex align-content-center flex-wrap">
        <img
          alt={rowData.logo ?? 'logo'}
          className="flex align-items-center justify-content-center max-w-full max-h-2rem h-2rem"
          onError={(e: React.SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = setting.defaultIcon))}
          src={`${encodeURI(rowData.logo ?? '')}`}
        />
      </div>
    );
  }, [setting.defaultIcon]);

  const inputBitsPerSecondTemplate = React.useCallback((rowData: StreamStatisticsResult) => {

    if (rowData.inputBitsPerSecond === undefined) return (<div>0</div>);

    const kbps = rowData.inputBitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);
    return (
      <div>
        {roundedKbps.toLocaleString('en-US')}
      </div>
    );
  }, []);

  const inputElapsedTimeTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return (
      <div>
        {rowData.inputElapsedTime?.split('.')[0]}
      </div>
    );
  }, []);

  const inputStartTimeTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return (
      <div>
        {formatJSONDateString(rowData.inputStartTime ?? '')}
      </div>
    );

  }, []);

  const dataSource = React.useMemo((): StreamStatisticsResult[] => {
    let data = [] as StreamStatisticsResult[];

    props.dataSource.forEach((item) => {
      if (data.findIndex((x) => x.m3UStreamId === item.m3UStreamId) === -1) {
        data.push(item);
      }

    });

    return data;
  }, [props.dataSource]);

  const streamCount = React.useCallback((rowData: StreamStatisticsResult) => {
    return (
      <div>
        {props.dataSource.filter((x) => x.m3UStreamId === rowData.m3UStreamId).length}
      </div>
    );
  }, [props.dataSource])

  const targetActionBodyTemplate = React.useCallback((rowData: StreamStatisticsResult) => {

    return (
      <div className="dataselector p-inputgroup align-items-center justify-content-center">
        <Button
          // className="p-button-danger"
          icon="pi pi-angle-right"
          onClick={async () => await onFailStream(rowData)}
          rounded
          text
          tooltip="Next Stream"
          tooltipOptions={getTopToolOptions} />
      </div>
    );
  }, [onFailStream]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      {
        bodyTemplate: imageBodyTemplate, field: 'icon', style: {
          maxWidth: '4rem',
          width: '4rem',
        } as React.CSSProperties,
      },

      {
        align: 'center',
        field: 'videoStreamId', header: 'Video Id'
        , style: {
          maxWidth: '4rem',
          width: '4rem',
        } as React.CSSProperties,
      },
      { field: 'videoStreamName', header: 'Name' },
      {
        align: 'center',
        field: 'rank', header: 'Rank', style: {
          maxWidth: '4rem',
          width: '4rem',
        } as React.CSSProperties,
      },

      {
        align: 'center',
        bodyTemplate: streamCount, field: 'Count', header: 'Count', style: {
          maxWidth: '4rem',
          width: '4rem',
        } as React.CSSProperties,
      },
      {
        align: 'center',
        bodyTemplate: inputBitsPerSecondTemplate, field: 'inputBitsPerSecond', header: 'Input kbps'
        , style: {
          maxWidth: '10rem',
          width: '10rem',
        } as React.CSSProperties,
      },
      {
        align: 'center',
        bodyTemplate: inputElapsedTimeTemplate, field: 'inputElapsedTime', header: 'Input Elapsed', style: {
          maxWidth: '10rem',
          width: '10rem',
        } as React.CSSProperties,
      },
      {
        align: 'center',
        bodyTemplate: inputStartTimeTemplate, field: 'inputStartTime', header: 'Input Start', style: {
          maxWidth: '10rem',
          width: '10rem',
        } as React.CSSProperties,
      },
      {
        align: 'center', bodyTemplate: targetActionBodyTemplate, field: 'Actions',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as React.CSSProperties,
      },
    ]
  }, [imageBodyTemplate, inputBitsPerSecondTemplate, inputElapsedTimeTemplate, inputStartTimeTemplate, streamCount, targetActionBodyTemplate]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
        <DataSelector
          columns={sourceColumns}
          dataSource={dataSource}
          emptyMessage="No Streams"
          globalSearchEnabled={false}
          id='StreamingServerStatusPanel'
          isLoading={props.isLoading}
          style={{ height: 'calc(50vh - 40px)' }}
        />
      </div>
    </>
  );
};

StreamingServerStatusPanel.displayName = 'Streaming Server Status';
StreamingServerStatusPanel.defaultProps = {};
type StreamingServerStatusPanelProps = {
  dataSource: StreamStatisticsResult[];
  isLoading: boolean;
}
export default React.memo(StreamingServerStatusPanel);

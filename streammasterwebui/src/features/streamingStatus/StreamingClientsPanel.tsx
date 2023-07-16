/* eslint-disable @typescript-eslint/no-unused-vars */

import React from 'react';
import { formatJSONDateString, getTopToolOptions } from '../../common/common';
import { Toast } from 'primereact/toast';
import { type VideoStreamDto } from '../../store/iptvApi';
import { type ChangeVideoStreamChannelRequest } from '../../store/iptvApi';
import { type FailClientRequest } from '../../store/iptvApi';
import { type StreamStatisticsResult } from '../../store/iptvApi';
import DataSelector from '../dataSelector/DataSelector';
import { type ColumnMeta } from '../dataSelector/DataSelectorTypes';
import { VideoStreamSelector } from '../../components/VideoStreamSelector';
import { Button } from 'primereact/button';
import * as Hub from "../../store/signlar_functions";

const StreamingClientsPanel = (props: StreamingClientsPanelProps) => {
  const toast = React.useRef<Toast>(null);
  const clientBitsPerSecondTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    if (rowData.clientBitsPerSecond === undefined) return <div />;

    const kbps = rowData.clientBitsPerSecond / 1000;
    const roundedKbps = Math.ceil(kbps);
    return (<div>{roundedKbps.toLocaleString('en-US')}</div >);
  }, []);


  const onChangeVideoStreamChannel = React.useCallback(async (playingVideoStreamId: number, newVideoStreamId: number) => {
    if (playingVideoStreamId === undefined || playingVideoStreamId < 1 ||
      newVideoStreamId === undefined || newVideoStreamId < 1
    ) {
      return;
    }

    var toSend = {} as ChangeVideoStreamChannelRequest;
    toSend.playingVideoStreamId = playingVideoStreamId;
    toSend.newVideoStreamId = newVideoStreamId;

    await Hub.ChangeVideoStreamChannel(toSend)
      .then(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Changed Client Channel`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });
        }
      }).catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Failed Client Channel`,
            life: 3000,
            severity: 'error',
            summary: 'Error'
          });
        }
      });

  }, []);


  const videoStreamTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return (
      <VideoStreamSelector
        onChange={async (e) => {
          await onChangeVideoStreamChannel(rowData.videoStreamId ?? 0, e.id);
        }}
        value={rowData.m3UStreamName}
      />
    );
  }, [onChangeVideoStreamChannel]);

  const clientStartTimeTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return (<div>{formatJSONDateString(rowData.clientStartTime ?? '')}</div>);
  }, []);

  const clientElapsedTimeTemplate = React.useCallback((rowData: StreamStatisticsResult) => {
    return (<div>{rowData.clientElapsedTime?.split('.')[0]}</div >);
  }, []);

  const onFailClient = React.useCallback(async (rowData: StreamStatisticsResult) => {
    if (!rowData.clientId || rowData.clientId === undefined || rowData.clientId === '') {
      return;
    }

    var toSend = {} as FailClientRequest;
    toSend.clientId = rowData.clientId;

    await Hub.FailClient(toSend)
      .then(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Failed Client`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });
        }
      }).catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Failed to Fail Client`,
            life: 3000,
            severity: 'error',
            summary: 'Error'
          });
        }
      });

  }, []);

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const targetActionBodyTemplate = React.useCallback((rowData: StreamStatisticsResult) => {

    return (
      <div className="dataselector p-inputgroup align-items-center justify-content-center">
        <Button
          className="p-button-danger"
          icon="pi pi-times"
          onClick={async () => await onFailClient(rowData)}
          rounded
          text
          tooltip="Fail Client"
          tooltipOptions={getTopToolOptions} />
      </div>
    );
  }, [onFailClient]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [

      {
        field: 'clientAgent', header: 'Client/User Agent', style: {
          maxWidth: '14rem',
          width: '14rem',
        } as React.CSSProperties,
      },

      {
        align: 'center', field: 'm3UStreamId', header: 'Video Id', style: {
          maxWidth: '4rem',
          width: '4rem',
        } as React.CSSProperties,
      },
      { field: 'm3UStreamName', header: 'Name' },
      {
        align: 'center',
        bodyTemplate: videoStreamTemplate, field: 'videoStreamTemplate', header: 'Video Stream', style: {
          maxWidth: '14rem',
          width: '14rem',
        } as React.CSSProperties,
      },
      {
        align: 'center', bodyTemplate: clientStartTimeTemplate, field: 'clientStartTime', header: 'Client Start', style: {
          maxWidth: '10rem',
          width: '10rem',
        } as React.CSSProperties,
      },
      {
        align: 'center', bodyTemplate: clientElapsedTimeTemplate, field: 'clientElapsedTime', header: 'Client Elapsed', style: {
          maxWidth: '10rem',
          width: '10rem',
        } as React.CSSProperties,
      },
      {
        align: 'center', bodyTemplate: clientBitsPerSecondTemplate, field: 'clientBitsPerSecond', header: 'Client kbps', style: {
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
  }, [clientBitsPerSecondTemplate, clientElapsedTimeTemplate, clientStartTimeTemplate, targetActionBodyTemplate, videoStreamTemplate]);

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
        <DataSelector
          columns={sourceColumns}
          dataSource={props.dataSource}
          emptyMessage="No Clients Streaming"
          globalSearchEnabled={false}
          id='StreamingServerStatusPanel'
          isLoading={props.isLoading}
          style={{ height: 'calc(50vh - 40px)' }}
        />
      </div>
    </>
  );
};


StreamingClientsPanel.displayName = 'Streaming Clients Panel';
StreamingClientsPanel.defaultProps = {};
type StreamingClientsPanelProps = {
  dataSource: StreamStatisticsResult[];
  isLoading: boolean;
}
export default React.memo(StreamingClientsPanel);

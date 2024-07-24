import { LinkButton } from '@components/buttons/LinkButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { GetVs } from '@lib/smAPI/Vs/VsCommands';
import { GetVsRequest, StreamGroupDto, StreamGroupProfile, V } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';

export interface VDataSelectorProperties {
  streamGroupDto: StreamGroupDto | null;
  streamGroupProfile?: StreamGroupProfile | null;
}

const VDataSelector = (props: VDataSelectorProperties) => {
  const [dataSource, setDataSource] = useState<V[]>([]);

  useEffect(() => {
    Logger.debug('VDataSelector useEffect', props.streamGroupDto);
    if (props.streamGroupDto === null) {
      GetVs({})
        .then((a) => {
          if (a !== undefined) {
            setDataSource(a);
          }
        })
        .catch((error) => {
          console.log('error', error);
        });
      return;
    }

    if (props.streamGroupProfile !== null && props.streamGroupProfile !== undefined) {
      GetVs({ StreamGroupId: props.streamGroupDto.Id, StreamGroupProfileId: props.streamGroupProfile.Id } as GetVsRequest)
        .then((a) => {
          if (a !== undefined) {
            setDataSource(a);
          }
        })
        .catch((error) => {
          console.log('error', error);
        });
      return;
    }

    GetVs({ StreamGroupId: props.streamGroupDto.Id } as GetVsRequest)
      .then((a) => {
        if (a !== undefined) {
          setDataSource(a);
        }
      })
      .catch((error) => {
        console.log('error', error);
      });
  }, [props.streamGroupDto, props.streamGroupProfile]);

  const nameTemplate = useCallback((v: V) => {
    return <div className="text-container pl-1">{v.Name}</div>;
  }, []);

  const realUrlTemplate = useCallback((v: V) => {
    return (
      <div className="sm-end-stuff">
        <div className="text-container pr-2">{v.RealUrl}</div>
        <LinkButton filled link={v.RealUrl} title="V Link" />
      </div>
    );
  }, []);

  const defaultRealUrlTemplate = useCallback((v: V) => {
    return (
      <div className="sm-end-stuff">
        <div className="text-container pr-2">{v.DefaultRealUrl}</div>
        <LinkButton filled link={v.DefaultRealUrl} title="Default ALL Profile V Link" />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: nameTemplate,
        field: 'Name',
        filter: true,
        sortable: true,
        width: 40
      },
      {
        field: 'StreamGroupProfileName',
        header: 'Profile Name',
        sortable: true,
        width: 22
      },
      {
        align: 'right',
        bodyTemplate: realUrlTemplate,
        field: 'RealUrl',
        width: 50
      },
      {
        align: 'right',
        bodyTemplate: defaultRealUrlTemplate,
        field: 'DefaultRealUrl',
        header: 'ALL Default Profile Url',
        width: 50
      }
    ],
    [nameTemplate, realUrlTemplate, defaultRealUrlTemplate]
  );

  return (
    <SMDataTable
      columns={columns}
      dataSource={dataSource}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Channels"
      enableClick
      enableExport={false}
      id="VDataSelector"
      lazy
      noSourceHeader
      onRowClick={(e: DataTableRowClickEvent) => {
        console.log('VDataSelector', e);
      }}
      style={{ height: '40vh' }}
    />
  );
};

VDataSelector.displayName = 'Stream Group Editor';

export default memo(VDataSelector);

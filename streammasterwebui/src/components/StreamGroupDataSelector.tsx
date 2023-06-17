import * as StreamMasterApi from '../store/iptvApi';
import { type CSSProperties } from "react";
import React from "react";
import DataSelector from "../features/dataSelector/DataSelector";
import { Toast } from 'primereact/toast';
import { type ColumnMeta } from '../features/dataSelector/DataSelectorTypes';
import StreamGroupAddDialog from './StreamGroupAddDialog';
import StreamGroupEditDialog from './StreamGroupEditDialog';
import StreamGroupDeleteDialog from './StreamGroupDeleteDialog';


const StreamGroupDataSelector = (props: StreamGroupDataSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const streamGroupsQuery = StreamMasterApi.useStreamGroupsGetStreamGroupsQuery();
  const [selectedStreamGroup, setSelectedStreamGroup] = React.useState<StreamMasterApi.StreamGroupDto>({} as StreamMasterApi.StreamGroupDto);

  const links = StreamMasterApi.useStreamGroupsGetStreamGroupLinksQuery(selectedStreamGroup.id);

  const m3uLink = React.useMemo(() => {
    if (links.data === undefined) {
      return '';
    }

    return links.data.m3ULink;
  }, [links.data]);

  React.useMemo(() => {
    if (selectedStreamGroup.id === undefined || !streamGroupsQuery.data) {
      return;
    }

    if (selectedStreamGroup.id) {
      const index = streamGroupsQuery.data.findIndex((e) => e.id === selectedStreamGroup.id);
      setSelectedStreamGroup({ ...streamGroupsQuery.data[index] });
    }

  }, [selectedStreamGroup.id, streamGroupsQuery.data]);

  const StreamGroupColumns = React.useMemo((): ColumnMeta[] => {
    return [
      {
        field: 'name',
        filter: true,
        header: 'Name',
        sortable: true,
        style: {
          width: '12rem',
        } as CSSProperties,
      },

      {
        field: 'id',
        fieldType: 'url',
      },
      {
        field: 'id',
        fieldType: 'epglink',
      },
      {
        field: 'id',
        fieldType: 'm3ulink',
      },
    ];
  }, []);

  const onSetStreamGroup = React.useCallback((data: StreamMasterApi.StreamGroupDto) => {
    if (!streamGroupsQuery?.data) {
      setSelectedStreamGroup({} as StreamMasterApi.StreamGroupDto);
      props.onSelectionChange?.({} as StreamMasterApi.StreamGroupDto);
      return;
    }

    const sg = streamGroupsQuery.data.find((x: StreamMasterApi.StreamGroupDto) => x.id === data.id);
    if (sg === null || sg === undefined) {
      setSelectedStreamGroup({} as StreamMasterApi.StreamGroupDto);
      props.onSelectionChange?.({} as StreamMasterApi.StreamGroupDto);
      return;
    }

    setSelectedStreamGroup(sg);
    props.onSelectionChange?.(sg);

  }, [props, streamGroupsQuery.data]);

  const sourceaddtionalHeaderTemplate = () => {
    return (
      <div className="streamGroupEditor grid w-full flex flex-nowrap justify-content-end align-items-center p-0">
        <div className="flex w-full w-full p-0 align-items-center justify-content-end">
          <div className="flex justify-content-end gap-2 align-items-center mr-2">

            <StreamGroupEditDialog
              onHide={(sg) => {
                if (sg !== undefined) {
                  setSelectedStreamGroup(sg);
                  props.onSelectionChange?.(sg);
                }
              }}
              value={selectedStreamGroup}
            />

            <StreamGroupAddDialog />

            <StreamGroupDeleteDialog value={selectedStreamGroup} />

          </div >
        </div>
      </div >
    );
  };

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <DataSelector
        columns={StreamGroupColumns}
        dataSource={streamGroupsQuery.data}
        headerRightTemplate={sourceaddtionalHeaderTemplate()}
        id={props.id + '-ds-source'}
        isLoading={streamGroupsQuery.isLoading}
        name="Stream Groups"
        onSelectionChange={(e) => {
          onSetStreamGroup(e as StreamMasterApi.StreamGroupDto);
        }
        }
        sortField='rank'
        style={{ height: 'calc(100vh - 40px)' }}
      />
    </>
  );
}

StreamGroupDataSelector.displayName = 'Stream Group Editor';
StreamGroupDataSelector.defaultProps = {
};

export type StreamGroupDataSelectorProps = {

  id: string;

  onSelectionChange?: (value: StreamMasterApi.StreamGroupDto | StreamMasterApi.StreamGroupDto[]) => void;
};

export default React.memo(StreamGroupDataSelector);

/* eslint-disable @typescript-eslint/no-unused-vars */

import { memo, useCallback, useMemo, useRef, useState, type CSSProperties } from "react";

import { Toast } from 'primereact/toast';

import StreamGroupAddDialog from '../../components/streamGroup/StreamGroupAddDialog';
import StreamGroupEditDialog from '../../components/streamGroup/StreamGroupEditDialog';
import StreamGroupDeleteDialog from '../../components/streamGroup/StreamGroupDeleteDialog';
import { type StreamGroupDto } from "../../store/iptvApi";
import { useStreamGroupsGetStreamGroupsQuery } from "../../store/iptvApi";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import DataSelector from "../../components/dataSelector/DataSelector";

const StreamGroupDataSelector = (props: StreamGroupDataSelectorProps) => {
  const toast = useRef<Toast>(null);

  // const streamGroupsQuery = useStreamGroupsGetStreamGroupsQuery({} as StreamGroupsGetStreamGroupsApiArg);

  const [selectedStreamGroup, setSelectedStreamGroup] = useState<StreamGroupDto>({} as StreamGroupDto);

  // useMemo(() => {
  //   if (selectedStreamGroup.id === undefined || !streamGroupsQuery.data || streamGroupsQuery.data.data === undefined) {
  //     return;
  //   }

  //   if (selectedStreamGroup.id) {
  //     const index = streamGroupsQuery.data.data.findIndex((e) => e.id === selectedStreamGroup.id);
  //     const newSG = { ...streamGroupsQuery.data.data[index] };
  //     setSelectedStreamGroup(newSG);
  //   }

  // }, [selectedStreamGroup.id, streamGroupsQuery.data]);

  const StreamGroupColumns = useMemo((): ColumnMeta[] => {
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

  const onSetStreamGroup = useCallback((data: StreamGroupDto) => {
    // if (!streamGroupsQuery?.data) {
    //   setSelectedStreamGroup({} as StreamGroupDto);
    //   props.onSelectionChange?.({} as StreamGroupDto);
    //   return;
    // }

    // const sg = streamGroupsQuery.data.data.find((x: StreamGroupDto) => x.id === data.id);
    // if (sg === null || sg === undefined) {
    //   setSelectedStreamGroup({} as StreamGroupDto);
    //   props.onSelectionChange?.({} as StreamGroupDto);
    //   return;
    // }

    // setSelectedStreamGroup(sg);
    // props.onSelectionChange?.(sg);

  }, []);

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
        headerName='Stream Groups'
        headerRightTemplate={sourceaddtionalHeaderTemplate()}
        id={props.id + '-ds-source'}
        onSelectionChange={(e) => {
          onSetStreamGroup(e as StreamGroupDto);
          props.onSelectionChange?.(e as StreamGroupDto);
        }
        }
        queryFilter={useStreamGroupsGetStreamGroupsQuery}
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
  onSelectionChange?: (value: StreamGroupDto) => void;
};

export default memo(StreamGroupDataSelector);

import { memo, useMemo, useRef, useState, type CSSProperties } from "react";

import { Toast } from 'primereact/toast';

import StreamGroupAddDialog from '../../components/streamGroup/StreamGroupAddDialog';
import StreamGroupEditDialog from '../../components/streamGroup/StreamGroupEditDialog';
import StreamGroupDeleteDialog from '../../components/streamGroup/StreamGroupDeleteDialog';
import { type StreamGroupDto } from "../../store/iptvApi";
import { useStreamGroupsGetStreamGroupsQuery } from "../../store/iptvApi";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import DataSelector from "../../components/dataSelector/DataSelector";
import { useStreamGroupToRemove } from "../../app/slices/useStreamGroupToRemove";


const StreamGroupDataSelector = (props: StreamGroupDataSelectorProps) => {
  const toast = useRef<Toast>(null);
  const { streamGroupToRemove } = useStreamGroupToRemove('StreamGroupDataSelector');

  const [selectedStreamGroup, setSelectedStreamGroup] = useState<StreamGroupDto>({} as StreamGroupDto);

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

            <StreamGroupDeleteDialog id='StreamGroupDataSelector' value={selectedStreamGroup} />

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
          props.onSelectionChange?.(e as StreamGroupDto);
        }
        }
        queryFilter={useStreamGroupsGetStreamGroupsQuery}
        streamToRemove={streamGroupToRemove}
        style={{ height: 'calc(100vh - 40px)' }}
      />
    </>
  );
}

StreamGroupDataSelector.displayName = 'Stream Group Editor';
StreamGroupDataSelector.defaultProps = {
};

export type StreamGroupDataSelectorProps = {
  readonly id: string;
  readonly onSelectionChange?: (value: StreamGroupDto) => void;
};

export default memo(StreamGroupDataSelector);

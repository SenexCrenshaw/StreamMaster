import { Toast } from 'primereact/toast';
import { memo, useMemo, useRef, type CSSProperties } from "react";
import { useSelectedStreamGroup } from "../../app/slices/useSelectedStreamGroup";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import StreamGroupAddDialog from '../../components/streamGroup/StreamGroupAddDialog';
import StreamGroupDeleteDialog from '../../components/streamGroup/StreamGroupDeleteDialog';
import StreamGroupEditDialog from '../../components/streamGroup/StreamGroupEditDialog';
import { useStreamGroupsGetStreamGroupsQuery, type StreamGroupDto } from "../../store/iptvApi";

export type StreamGroupDataSelectorProps = {
  readonly id: string;
};

const StreamGroupDataSelector = ({ id }: StreamGroupDataSelectorProps) => {
  const toast = useRef<Toast>(null);
  const { setSelectedStreamGroup } = useSelectedStreamGroup(id);

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
        field: 'streamCount',
        header: '#',
        style: {
          maxWidth: '1rem',
          width: '1rem',
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

            <StreamGroupEditDialog id={id} />

            <StreamGroupAddDialog />

            <StreamGroupDeleteDialog id={id} />

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
        id={id + '-ds-source'}
        onSelectionChange={(e) => {
          setSelectedStreamGroup(e as StreamGroupDto);
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


export default memo(StreamGroupDataSelector);

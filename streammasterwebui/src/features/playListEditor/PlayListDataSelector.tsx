import { memo, useCallback, useEffect, useMemo, type CSSProperties } from "react";
import { useSelectedChannelGroups } from "../../app/slices/useSelectedChannelGroups";
import { useShowHidden } from "../../app/slices/useShowHidden";
import { isEmptyObject } from "../../common/common";
import ChannelGroupAddDialog from "../../components/channelGroups/ChannelGroupAddDialog";
import ChannelGroupDeleteDialog from "../../components/channelGroups/ChannelGroupDeleteDialog";
import ChannelGroupEditDialog from "../../components/channelGroups/ChannelGroupEditDialog";
import ChannelGroupVisibleDialog from "../../components/channelGroups/ChannelGroupVisibleDialog";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { TriSelect } from "../../components/selectors/TriSelect";
import { useChannelGroupsGetChannelGroupsQuery, type ChannelGroupDto } from "../../store/iptvApi";

export type PlayListDataSelectorProps = {
  readonly hideAddRemoveControls?: boolean;
  readonly hideControls?: boolean;
  readonly id: string;
  readonly maxHeight?: number;
  readonly name?: string;
  readonly useReadOnly?: boolean;
};


const PlayListDataSelector = (props: PlayListDataSelectorProps) => {
  const dataKey = props.id + '-PlayListDataSelector';
  const { showHidden, setShowHidden } = useShowHidden(dataKey);

  const { setSelectedChannelGroups } = useSelectedChannelGroups(props.id);

  useEffect(() => {
    if (showHidden === undefined) {
      setShowHidden(null);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [showHidden]);


  const sourceActionBodyTemplate = useCallback((data: ChannelGroupDto) => (

    <div className='flex p-0 justify-content-end align-items-center'>

      <div hidden={data.isReadOnly === true && props.useReadOnly}>
        <ChannelGroupDeleteDialog cgid={props.id} iconFilled={false} id={dataKey} value={data} />
      </div>

      <ChannelGroupEditDialog cgid={props.id} value={data} />
      <ChannelGroupVisibleDialog cgid={props.id} id={dataKey} skipOverLayer value={data} />

    </div>

  ), [dataKey, props.id, props.useReadOnly]);

  const sourceColumns = useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
      {
        field: 'streams', fieldType: 'streams', header: "Streams (active/total)",
        style: {
          maxWidth: '6rem',
          width: '6rem',
        } as CSSProperties,
      },
      {
        align: 'right',
        bodyTemplate: sourceActionBodyTemplate, field: 'isHidden', fieldType: 'isHidden', header: 'Actions',
        style: {
          maxWidth: '8rem',
          width: '8rem',
        } as CSSProperties,
      },
    ]
  }, [sourceActionBodyTemplate]);

  const sourceRightHeaderTemplate = useCallback(() => {

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1">

        {props.hideControls !== true &&
          <>
            <TriSelect dataKey={dataKey} />
            <ChannelGroupVisibleDialog cgid={props.id} id={dataKey} skipOverLayer={false} />
            <ChannelGroupDeleteDialog cgid={props.id} iconFilled id={dataKey} />
          </>
        }

        <ChannelGroupAddDialog />
      </div>
    );
  }, [props.hideControls, props.id, dataKey]);

  return (

    <DataSelector
      columns={sourceColumns}
      emptyMessage="No Channel Groups"
      headerName={props.name === undefined ? 'Playlist' : props.name}
      headerRightTemplate={props.hideAddRemoveControls === true ? null : sourceRightHeaderTemplate()}
      hideControls={props.hideControls}
      id={dataKey}
      onSelectionChange={(e) => {
        if (!isEmptyObject(e)) {
          setSelectedChannelGroups(e as ChannelGroupDto[]);
        } else {
          setSelectedChannelGroups([]);
        }
      }}
      queryFilter={useChannelGroupsGetChannelGroupsQuery}
      selectionMode='multiple'
      style={{
        height: props.maxHeight !== null ? props.maxHeight : 'calc(100vh - 40px)',
      }}
    />

  );
};

PlayListDataSelector.displayName = 'Play List Editor';
PlayListDataSelector.defaultProps = {
  hideAddRemoveControls: false,
  hideControls: false,
  maxHeight: null,
  name: 'Playlist',
  useReadOnly: true

};


export default memo(PlayListDataSelector);

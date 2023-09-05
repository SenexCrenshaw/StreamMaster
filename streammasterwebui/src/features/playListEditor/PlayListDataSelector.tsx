import { memo, useCallback, useMemo, useState, type CSSProperties } from "react";
import { isEmptyObject } from "../../common/common";
import { getTopToolOptions } from "../../common/common";
import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import { type ChannelGroupDto } from "../../store/iptvApi";
import { useChannelGroupsGetChannelGroupsQuery } from "../../store/iptvApi";
import ChannelGroupAddDialog from "../../components/channelGroups/ChannelGroupAddDialog";
import ChannelGroupDeleteDialog from "../../components/channelGroups/ChannelGroupDeleteDialog";
import ChannelGroupEditDialog from "../../components/channelGroups/ChannelGroupEditDialog";
import ChannelGroupVisibleDialog from "../../components/channelGroups/ChannelGroupVisibleDialog";
import DataSelector from "../../components/dataSelector/DataSelector";
import { type ColumnMeta } from "../../components/dataSelector/DataSelectorTypes";
import { useChannelGroupToRemove } from "../../app/slices/useChannelGroupToRemove";
import { useShowHidden } from "../../app/slices/useShowHidden";


const PlayListDataSelector = (props: PlayListDataSelectorProps) => {
  const dataKey = props.id + '-PlayListDataSelector';
  const { showHidden, setShowHidden } = useShowHidden(dataKey);
  const [selectedChannelGroups, setSelectedChannelGroups] = useState<ChannelGroupDto[]>([] as ChannelGroupDto[]);
  const { channelGroupToRemove } = useChannelGroupToRemove(dataKey);



  const onSelectionChange = (data: ChannelGroupDto[]) => {
    setSelectedChannelGroups(data);
    props.onSelectionChange?.(data);
  };

  // const onDelete = useCallback((results: string[] | undefined) => {
  //   if (results === undefined) {
  //     return;
  //   }

  //   const newSelectedChannelGroups = selectedChannelGroups.filter(
  //     group => !results.includes(group.name)
  //   );

  //   onSelectionChange(newSelectedChannelGroups);

  //
  // }, []);

  const sourceActionBodyTemplate = useCallback((data: ChannelGroupDto) => (

    <div className='flex p-0 justify-content-end align-items-center'>

      <div hidden={data.isReadOnly === true && props.useReadOnly}>
        <ChannelGroupDeleteDialog iconFilled={false} id={dataKey} values={[data]} />
      </div>

      <ChannelGroupEditDialog value={data} />

      <ChannelGroupVisibleDialog iconFilled={false} skipOverLayer value={[data]} />

    </div>

  ), [dataKey, props.useReadOnly]);

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
    const getToolTip = (value: boolean | null | undefined) => {
      if (value === null) {
        return 'Show All';
      }

      if (value === true) {
        return 'Show Visible';
      }

      return 'Show Hidden';
    }

    return (
      <div className="flex justify-content-end align-items-center w-full gap-1">

        {props.hideControls !== true &&
          <>
            <TriStateCheckbox
              onChange={(e: TriStateCheckboxChangeEvent) => {
                setShowHidden(e.value);
              }}
              tooltip={getToolTip(showHidden)}
              tooltipOptions={getTopToolOptions}
              value={showHidden} />

            <ChannelGroupVisibleDialog value={selectedChannelGroups} />
            <ChannelGroupDeleteDialog iconFilled id={dataKey} values={selectedChannelGroups} />
          </>
        }

        <ChannelGroupAddDialog />
      </div>
    );
  }, [props.hideControls, showHidden, selectedChannelGroups, dataKey, setShowHidden]);


  return (

    <DataSelector
      columns={sourceColumns}
      emptyMessage="No Groups"
      headerName={props.name === undefined ? 'Playlist' : props.name}
      headerRightTemplate={props.hideAddRemoveControls === true ? null : sourceRightHeaderTemplate()}
      hideControls={props.hideControls}
      id={dataKey}
      onSelectionChange={(e) => {
        if (!isEmptyObject(e)) {
          onSelectionChange(e as ChannelGroupDto[]);
        } else {
          onSelectionChange([] as ChannelGroupDto[]);
        }
      }}
      queryFilter={useChannelGroupsGetChannelGroupsQuery}
      selectionMode='multiple'
      streamToRemove={channelGroupToRemove}
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

export type PlayListDataSelectorProps = {
  hideAddRemoveControls?: boolean;
  hideControls?: boolean;
  id: string;
  maxHeight?: number;
  name?: string;
  onSelectionChange?: (value: ChannelGroupDto | ChannelGroupDto[]) => void;
  useReadOnly?: boolean;
};

export default memo(PlayListDataSelector);

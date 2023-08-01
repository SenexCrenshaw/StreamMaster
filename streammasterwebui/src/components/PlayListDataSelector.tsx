import { type CSSProperties } from "react";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';

import DataSelector from "../features/dataSelector/DataSelector";
import { getTopToolOptions } from "../common/common";
import { Toast } from 'primereact/toast';

import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";

import ChannelGroupAddDialog from "./ChannelGroupAddDialog";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import ChannelGroupDeleteDialog from "./ChannelGroupDeleteDialog";
import ChannelGroupVisibleDialog from "./ChannelGroupVisibleDialog";
import ChannelGroupEditDialog from "./ChannelGroupEditDialog";
import { useLocalStorage } from "primereact/hooks";

const PlayListDataSelector = (props: PlayListDataSelectorProps) => {

  const toast = React.useRef<Toast>(null);
  const [dataSource, setDataSource] = React.useState([] as StreamMasterApi.ChannelGroupDto[]);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-PlayListDataSelector-showHidden');
  const [selectedChannelGroups, setSelectedChannelGroups] = useLocalStorage<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[], props.id + '-PlayListDataSelector-selectedChannelGroups');

  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery();
  StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();

  const isLoading = React.useMemo(() => {

    if (channelGroupsQuery.isLoading) {
      return true;
    }

    if (selectedChannelGroups === undefined) {
      return true;
    }

    return false;
  }, [channelGroupsQuery.isLoading, selectedChannelGroups]);

  React.useEffect(() => {
    if (!selectedChannelGroups || selectedChannelGroups.length === 0) {
      return;
    }

    if (dataSource.length === 0) {
      return;
    }

    const ids = selectedChannelGroups.map((item) => item.id);
    const newSelectedChannelGroups = dataSource.filter((item) => ids.includes(item.id));

    setSelectedChannelGroups(newSelectedChannelGroups);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [dataSource]);

  React.useEffect(() => {
    if (channelGroupsQuery.data === undefined) {
      return;
    }

    if (props.hideControls === true) {
      setDataSource(channelGroupsQuery.data?.filter((item) => item.isHidden !== true));
    } else {
      setDataSource(channelGroupsQuery.data);
    }

  }, [channelGroupsQuery.data, props.hideControls]);


  const sourceActionBodyTemplate = React.useCallback((data: StreamMasterApi.ChannelGroupDto) => (
    <div className='flex p-0 justify-content-end align-items-center'>
      <div hidden={data.isReadOnly === true && props.useReadOnly}>
        <ChannelGroupDeleteDialog iconFilled={false} value={[data]} />
      </div>

      <ChannelGroupEditDialog value={data} />

      <ChannelGroupVisibleDialog iconFilled={false} skipOverLayer value={[data]} />

    </div>
  ), [props.useReadOnly]);

  const onsetSelectedChannelGroups = React.useCallback((selectedData: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => {

    if (Array.isArray(selectedData)) {
      const newDatas = selectedData.filter((cg) => cg.id !== undefined);
      setSelectedChannelGroups(newDatas);
      props.onSelectionChange?.(newDatas);
    } else {
      setSelectedChannelGroups([selectedData]);
      props.onSelectionChange?.([selectedData]);
    }

  }, [props, setSelectedChannelGroups]);

  const sourceRightHeaderTemplate = React.useCallback(() => {
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
            <ChannelGroupDeleteDialog value={selectedChannelGroups} />
          </>
        }

        <ChannelGroupAddDialog />
      </div>
    );
  }, [showHidden, props.hideControls, selectedChannelGroups, setShowHidden]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
      {
        field: 'name', fieldType: 'streams', header: "Streams (active/total)",
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

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <DataSelector
        columns={sourceColumns}
        dataSource={dataSource}
        emptyMessage="No Groups"
        enableState={props.enableState}
        enableVirtualScroll
        headerRightTemplate={props.hideAddRemoveControls === true ? null : sourceRightHeaderTemplate()}
        hideControls={props.hideControls}
        id={props.id + 'DataSelector'}
        isLoading={isLoading}
        name={props.name === undefined ? 'Playlist' : props.name}
        onSelectionChange={(e) => {
          onsetSelectedChannelGroups(e as StreamMasterApi.ChannelGroupDto[]);
        }
        }
        rightColSize={5}
        selection={selectedChannelGroups}
        selectionMode='multiple'
        showHidden={showHidden}
        showSkeleton={isLoading}
        sortField='rank'
        style={{
          height: props.maxHeight !== null ? props.maxHeight : 'calc(100vh - 40px)',
        }}

      />
    </>
  );
};

PlayListDataSelector.displayName = 'Play List Editor';
PlayListDataSelector.defaultProps = {
  enableState: false,
  hideAddRemoveControls: false,
  hideControls: false,
  maxHeight: null,
  name: 'Playlist',
  useReadOnly: true
};

export type PlayListDataSelectorProps = {
  enableState?: boolean | undefined;
  hideAddRemoveControls?: boolean;
  hideControls?: boolean;
  id: string;
  maxHeight?: number;
  name?: string;
  onSelectionChange?: (value: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => void;
  // selectChannelGroups?: StreamMasterApi.ChannelGroupDto[] | null
  useReadOnly?: boolean;
};

export default React.memo(PlayListDataSelector);

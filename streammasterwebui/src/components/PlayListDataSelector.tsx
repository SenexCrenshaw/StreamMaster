import { type CSSProperties } from "react";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';

import DataSelector from "../features/dataSelector/DataSelector";
import { getTopToolOptions } from "../common/common";
import { Toast } from 'primereact/toast';

import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";
import M3UFileAddDialog from "./M3UFileAddDialog";

import ChannelGroupAddDialog from "./ChannelGroupAddDialog";
import { type ColumnMeta } from "../features/dataSelector/DataSelectorTypes";
import ChannelGroupDeleteDialog from "./ChannelGroupDeleteDialog";
import ChannelGroupVisibleDialog from "./ChannelGroupVisibleDialog";
import ChannelGroupEditDialog from "./ChannelGroupEditDialog";
import M3UFilesSelector from "./M3UFilesSelector";
import { useLocalStorage } from "primereact/hooks";

const PlayListDataSelector = (props: PlayListDataSelectorProps) => {

  const toast = React.useRef<Toast>(null);

  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-PlayListDataSelector-showHidden');

  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);

  const [selectedM3UFile, setSelectedM3UFile] = useLocalStorage<StreamMasterApi.M3UFilesDto>({ id: 0, name: 'All' } as StreamMasterApi.M3UFilesDto, props.id + '-PlayListDataSelector-selectedM3UFile');

  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery();
  const videoStreamsQuery = StreamMasterApi.useVideoStreamsGetVideoStreamsQuery();

  // const [sourceEnableBulk, setSourceEnableBulk] = useLocalStorage(false, props.id + '-PlayListDataSelector-sourceEnableBulk');


  // React.useEffect(() => {
  //   const callback = (event: KeyboardEvent) => {

  //     if ((event.ctrlKey) && event.code === 'KeyB') {
  //       event.preventDefault();
  //       setSourceEnableBulk(!sourceEnableBulk);
  //     }

  //   };

  //   document.addEventListener('keydown', callback);
  //   return () => {
  //     document.removeEventListener('keydown', callback);
  //   };
  // }, [setSourceEnableBulk, sourceEnableBulk]);

  React.useEffect(() => {
    if (!props.selectChannelGroups) {
      return;
    }

    console.log("PlayListDataSelector:useEffect:props.selectChannelGroups", props.selectChannelGroups)
    setSelectedChannelGroups(props.selectChannelGroups);
  }, [props.selectChannelGroups]);

  const channelGroupDelete = React.useCallback((ids: number[]) => {
    const test = selectedChannelGroups.filter((item) => !ids.includes(item.id));
    setSelectedChannelGroups(test);
  }, [selectedChannelGroups]);

  const sourceActionBodyTemplate = React.useCallback((data: StreamMasterApi.ChannelGroupDto) => (
    <div className='flex p-0 justify-content-center align-items-center'>
      <div hidden={data.isReadOnly === true && props.useReadOnly}>
        <ChannelGroupDeleteDialog iconFilled={false} onChange={channelGroupDelete} value={[data]} />
      </div>

      <ChannelGroupEditDialog value={data} />

      <ChannelGroupVisibleDialog iconFilled={false} skipOverLayer value={[data]} />

    </div>
  ), [channelGroupDelete, props.useReadOnly]);

  const sourceHeaderLeftTemplate = () => {
    return (
      <div className="p-inputgroup border-round"
        style={{
          border: '1px solid #FE7600',
        }}
      >
        <div className="flex col-2 p-0 m-0">
          <M3UFileAddDialog
            onClick={
              (e) => {
                setSelectedM3UFile(e);
                props.onM3UFileChange?.(e);
              }
            }
          />
        </div>

        <div className='flex col-10 justify-content-start align-items-center p-0 m-0 pl-1'>
          {/* {selectedM3UFile?.name ?? ''} */}
          <M3UFilesSelector id={props.id} onChange={(e) => setSelectedM3UFile(e)} value={selectedM3UFile} />
        </div>

      </div >
    );
  }

  const onsetSelectedChannelGroups = React.useCallback((selectedData: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => {

    if (Array.isArray(selectedData)) {
      const newDatas = selectedData.filter((cg) => cg.id !== undefined);
      setSelectedChannelGroups(newDatas);
      props.onSelectionChange?.(newDatas);
    } else {
      setSelectedChannelGroups([selectedData]);
      // if (sourceEnableBulk) {
      props.onSelectionChange?.([selectedData]);
      // } else {
      //   props.onSelectionChange?.(selectedData);
      // }
    }

  }, [props]);



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
            <ChannelGroupDeleteDialog onChange={channelGroupDelete} value={selectedChannelGroups} />
          </>
        }

        <ChannelGroupAddDialog />
      </div>
    );
  }, [showHidden, props.hideControls, selectedChannelGroups, channelGroupDelete, setShowHidden]);

  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      {
        field: 'name', filter: true, sortable: true, style: {
          flexGrow: 1,
          flexShrink: 1,
        } as CSSProperties,
      },
      {
        field: 'name', fieldType: 'streams', header: "Streams (acitve/count)", sortable: true,
        style: {
          maxWidth: '4rem',
          width: '4rem',
        } as CSSProperties,
      },
      {
        bodyTemplate: sourceActionBodyTemplate, field: 'isHidden', fieldType: 'isHidden', sortable: true,
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
        dataSource={props.hideControls === true ? channelGroupsQuery.data?.filter((item) => item.isHidden !== true) : channelGroupsQuery.data}
        emptyMessage="No Groups"
        headerLeftTemplate={props.hideControls === true ? null : sourceHeaderLeftTemplate()}
        headerRightTemplate={props.hideAddRemoveControls === true ? null : sourceRightHeaderTemplate()}
        id={props.id + 'DataSelector'}
        isLoading={channelGroupsQuery.isLoading}
        m3uFileId={selectedM3UFile?.id}
        name={props.name === undefined ? 'Playlist' : props.name}
        onSelectionChange={(e) => {
          onsetSelectedChannelGroups(e as StreamMasterApi.ChannelGroupDto[]);
        }
        }
        rightColSize={5}
        selection={selectedChannelGroups}
        selectionMode='single'
        showHidden={showHidden}
        showSkeleton={
          channelGroupsQuery.isLoading ||
          videoStreamsQuery.isLoading ||
          selectedM3UFile === undefined ||
          selectedM3UFile.name === undefined
          // sourceEnableBulk === null ||
          // sourceEnableBulk === undefined
        }
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
  onM3UFileChange?: (value: StreamMasterApi.M3UFilesDto) => void;
  onSelectionChange?: (value: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => void;
  selectChannelGroups?: StreamMasterApi.ChannelGroupDto[] | null
  useReadOnly?: boolean;
};

export default React.memo(PlayListDataSelector);

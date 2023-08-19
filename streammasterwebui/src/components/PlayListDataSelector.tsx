
import { type CSSProperties } from "react";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';

import { type DataTableFilterMetaData } from "../common/common";
import { getTopToolOptions } from "../common/common";

import { type TriStateCheckboxChangeEvent } from "primereact/tristatecheckbox";
import { TriStateCheckbox } from "primereact/tristatecheckbox";

import ChannelGroupAddDialog from "./ChannelGroupAddDialog";
import { type ColumnMeta } from '../features/dataSelector2/DataSelectorTypes2';
import ChannelGroupDeleteDialog from "./ChannelGroupDeleteDialog";
import ChannelGroupVisibleDialog from "./ChannelGroupVisibleDialog";
import ChannelGroupEditDialog from "./ChannelGroupEditDialog";
import { useLocalStorage } from "primereact/hooks";
import DataSelector2 from "../features/dataSelector2/DataSelector2";

import { type DataTableFilterEvent } from "primereact/datatable";

const PlayListDataSelector = (props: PlayListDataSelectorProps) => {
  const id = props.id + '-PlayListDataSelector';

  const [dataSource, setDataSource] = React.useState({} as StreamMasterApi.PagedResponseOfChannelGroupDto);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, id + '-showHidden');

  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);

  const [pageSize, setPageSize] = React.useState<number>(25);
  const [pageNumber, setPageNumber] = React.useState<number>(1);
  const [filters, setFilters] = React.useState<string>('');
  const [orderBy, setOrderBy] = React.useState<string>('name');

  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery({ jsonFiltersString: filters, orderBy: orderBy ?? 'name', pageNumber: pageNumber === 0 ? 25 : pageNumber, pageSize: pageSize } as StreamMasterApi.ChannelGroupsGetChannelGroupsApiArg);

  React.useEffect(() => {
    if (channelGroupsQuery.data === undefined) {
      return;
    }

    if (!channelGroupsQuery.data?.data) {
      return;
    }

    setDataSource(channelGroupsQuery.data);

  }, [channelGroupsQuery.data]);

  const onNameChange = React.useCallback(() => {
    // console.debug('PlayListDataSelector: onSelectionChange: ' + newName);
  }, []);

  const onSelectionChange = React.useCallback((data: StreamMasterApi.ChannelGroupDto[]) => {
    setSelectedChannelGroups(data);
    props.onSelectionChange?.(data);
  }, [props]);

  const onDelete = React.useCallback((results: string[] | undefined) => {
    if (results === undefined) {
      return;
    }

    const newSelectedChannelGroups = selectedChannelGroups.filter(
      group => !results.includes(group.name)
    );

    onSelectionChange(newSelectedChannelGroups);

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const sourceActionBodyTemplate = React.useCallback((data: StreamMasterApi.ChannelGroupDto) => (

    <div className='flex p-0 justify-content-end align-items-center'>

      <div hidden={data.isReadOnly === true && props.useReadOnly}>
        <ChannelGroupDeleteDialog iconFilled={false} onDelete={onDelete} value={[data]} />
      </div>

      <ChannelGroupEditDialog onClose={onNameChange} value={data} />

      <ChannelGroupVisibleDialog iconFilled={false} skipOverLayer value={[data]} />

    </div>

  ), [onDelete, onNameChange, props.useReadOnly]);

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

  const setFilter = React.useCallback((toFilter: DataTableFilterEvent): DataTableFilterMetaData[] => {

    if (toFilter === undefined || toFilter.filters === undefined) {
      return [] as DataTableFilterMetaData[];
    }

    const retData = [] as DataTableFilterMetaData[];
    Object.keys(toFilter.filters).forEach((key) => {
      const value = toFilter.filters[key] as DataTableFilterMetaData;
      if (value.value === null || value.value === undefined || value.value === '') {
        return;
      }

      const newValue = { ...value } as DataTableFilterMetaData;
      newValue.fieldName = key;
      retData.push(newValue);
    });


    setFilters(JSON.stringify(retData));
    return retData;
  }, []);

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
            <ChannelGroupDeleteDialog onDelete={onDelete} value={selectedChannelGroups} />
          </>
        }

        <ChannelGroupAddDialog />
      </div>
    );
  }, [props.hideControls, showHidden, selectedChannelGroups, onDelete, setShowHidden]);


  return (

    <DataSelector2
      columns={sourceColumns}
      dataSource={dataSource}
      emptyMessage="No Groups"
      headerRightTemplate={props.hideAddRemoveControls === true ? null : sourceRightHeaderTemplate()}
      hideControls={props.hideControls}
      id={id + 'DataSelector'}
      isLoading={channelGroupsQuery.isLoading || channelGroupsQuery.isFetching}
      name={props.name === undefined ? 'Playlist' : props.name}
      onFilter={(filterInfo) => {
        setFilter(filterInfo as DataTableFilterEvent);
      }}

      onPage={(pageInfo) => {

        if (pageInfo.page !== undefined) {
          setPageNumber(pageInfo.page + 1);
        }

        if (pageInfo.rows !== undefined) {
          setPageSize(pageInfo.rows);
        }
      }}

      onSelectionChange={(e) => {
        onSelectionChange(e as StreamMasterApi.ChannelGroupDto[]);
      }}

      onSort={(sortInfo) => {

        if (sortInfo.sortField !== null && sortInfo.sortField !== undefined) {
          if (sortInfo.sortOrder === 1) {
            setOrderBy(sortInfo.sortField + " asc");
          }
          else {
            setOrderBy(sortInfo.sortField + " desc");
          }
        }

      }}
      rightColSize={5}
      selectionMode='multiple'
      showHidden={showHidden}
      style={{
        height: props.maxHeight !== null ? props.maxHeight : 'calc(100vh - 40px)',
      }}
    />

  );
};

PlayListDataSelector.displayName = 'Play List Editor';
PlayListDataSelector.defaultProps = {
  // enableState: false,
  hideAddRemoveControls: false,
  hideControls: false,
  maxHeight: null,
  name: 'Playlist',
  useReadOnly: true
};

export type PlayListDataSelectorProps = {
  // enableState?: boolean | undefined;
  hideAddRemoveControls?: boolean;
  hideControls?: boolean;
  id: string;
  maxHeight?: number;
  name?: string;
  onSelectionChange?: (value: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => void;
  useReadOnly?: boolean;
};

export default React.memo(PlayListDataSelector);

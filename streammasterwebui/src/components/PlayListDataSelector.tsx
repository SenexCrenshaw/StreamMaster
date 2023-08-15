/* eslint-disable @typescript-eslint/no-unused-vars */

import { type CSSProperties } from "react";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';

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
import DataSelector2 from "../features/dataSelector2/DataSelector2";

import { type DataTableFilterMeta } from "primereact/datatable";
import { type DataTableFilterEvent } from "primereact/datatable";

const PlayListDataSelector = (props: PlayListDataSelectorProps) => {

  const toast = React.useRef<Toast>(null);
  const [dataSource, setDataSource] = React.useState({} as StreamMasterApi.PagedResponseOfChannelGroupDto);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-PlayListDataSelector-showHidden');

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
    const tosend = [] as DataTableFilterMetaData[];
    if (toFilter === undefined || toFilter.filters === undefined) {
      return [] as DataTableFilterMetaData[];
    }

    Object.keys(toFilter.filters).forEach((key) => {
      const value = toFilter.filters[key] as DataTableFilterMetaData;
      if (value.value === null || value.value === undefined || value.value === '') {
        return;
      }

      const newValue = { ...value } as DataTableFilterMetaData;
      newValue.fieldName = key;
      newValue.valueType = typeof value.value;
      tosend.push(newValue);
    });

    console.debug('PlayListDataSelector onFilter', tosend)
    setFilters(JSON.stringify(tosend));
    return tosend;
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
            <ChannelGroupDeleteDialog value={selectedChannelGroups} />
          </>
        }

        <ChannelGroupAddDialog />
      </div>
    );
  }, [props.hideControls, showHidden, selectedChannelGroups, setShowHidden]);


  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <DataSelector2
        columns={sourceColumns}
        dataSource={dataSource}
        emptyMessage="No Groups"
        enableState={props.enableState}
        headerRightTemplate={props.hideAddRemoveControls === true ? null : sourceRightHeaderTemplate()}
        hideControls={props.hideControls}
        id={props.id + 'DataSelector'}
        isLoading={channelGroupsQuery.isLoading}
        name={props.name === undefined ? 'Playlist' : props.name}
        onFilter={(filterInfo) => {
          setFilter(filterInfo as DataTableFilterEvent);
        }}

        onPage={(pageInfo) => {
          console.debug(pageInfo.page, pageInfo.first, pageInfo.rows, pageInfo.pageCount, pageInfo.pageCount);
          if (pageInfo.page !== undefined) {
            setPageNumber(pageInfo.page + 1);
          }

          if (pageInfo.rows !== undefined) {
            setPageSize(pageInfo.rows);
          }
        }}

        onSelectionChange={(e) => {
          setSelectedChannelGroups(e as StreamMasterApi.ChannelGroupDto[]);
          props.onSelectionChange?.(e as StreamMasterApi.ChannelGroupDto[]);
        }}

        onSetSourceFilters={(filterInfo) => setFilter({ filters: filterInfo } as DataTableFilterEvent)}
        onSort={(sortInfo) => {
          console.debug('PlayListDataSelector onSort', sortInfo);
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
        showSkeleton={channelGroupsQuery.isLoading}
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

type DataTableFilterMetaData = {
  fieldName: string;
  matchMode: 'between' | 'contains' | 'custom' | 'dateAfter' | 'dateBefore' | 'dateIs' | 'dateIsNot' | 'endsWith' | 'equals' | 'gt' | 'gte' | 'in' | 'lt' | 'lte' | 'notContains' | 'notEquals' | 'startsWith' | undefined;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  value: any;
  valueType: string;
}

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

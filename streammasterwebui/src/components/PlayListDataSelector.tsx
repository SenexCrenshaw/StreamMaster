/* eslint-disable @typescript-eslint/no-explicit-any */
/* eslint-disable @typescript-eslint/no-unused-vars */
import { type CSSProperties } from "react";
import React from "react";
import * as StreamMasterApi from '../store/iptvApi';

import { convertFiltersToQueryString, getTopToolOptions } from "../common/common";
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

import { notDeepStrictEqual } from "assert";
import { normalizeContent } from "video.js/dist/types/utils/dom";
import { isStringObject } from "util/types";
import { type DataTableFilterEvent } from "primereact/datatable";
import { DataTableFilterMeta } from "primereact/datatable";

const PlayListDataSelector = (props: PlayListDataSelectorProps) => {

  const toast = React.useRef<Toast>(null);
  const [dataSource, setDataSource] = React.useState({} as StreamMasterApi.PagedResponseOfChannelGroupDto);
  const [showHidden, setShowHidden] = useLocalStorage<boolean | null | undefined>(undefined, props.id + '-PlayListDataSelector-showHidden');

  const [selectedChannelGroups, setSelectedChannelGroups] = React.useState<StreamMasterApi.ChannelGroupDto[]>([] as StreamMasterApi.ChannelGroupDto[]);

  const [pageSize, setPageSize] = React.useState<number>(25);
  const [pageNumber, setPageNumber] = React.useState<number>(1);
  const [filters, setFilters] = React.useState<string>('');
  const [filtersMeta, setFiltersMeta] = React.useState<DataTableFilterMetaData[]>([] as DataTableFilterMetaData[]);

  const [orderBy, setOrderBy] = React.useState<string>('name');

  const channelGroupsQuery = StreamMasterApi.useChannelGroupsGetChannelGroupsQuery({ jsonFiltersString: filters, orderBy: orderBy ?? 'name', pageNumber: pageNumber === 0 ? 25 : pageNumber, pageSize: pageSize } as StreamMasterApi.ChannelGroupsGetChannelGroupsApiArg);

  // const isLoading = React.useMemo(() => {

  //   if (channelGroupsQuery.isLoading) {
  //     return true;
  //   }

  //   // if (selectedChannelGroups === undefined) {
  //   //   return true;
  //   // }

  //   return false;
  // }, [channelGroupsQuery.isLoading, selectedChannelGroups]);

  // React.useEffect(() => {
  //   if (!selectedChannelGroups || selectedChannelGroups.length === 0) {
  //     return;
  //   }

  //   if (!dataSource.data || dataSource.data.length === 0) {
  //     return;
  //   }

  //   const ids = selectedChannelGroups.map((item) => item.id);
  //   const newSelectedChannelGroups = dataSource.data.filter((item) => ids.includes(item.id));

  //   setSelectedChannelGroups(newSelectedChannelGroups);
  //   // eslint-disable-next-line react-hooks/exhaustive-deps
  // }, [dataSource]);

  React.useEffect(() => {
    if (channelGroupsQuery.data === undefined) {
      return;
    }

    console.debug('PlayListDataSelector channelGroupsQuery.data', channelGroupsQuery.data);
    console.debug('PlayListDataSelector channelGroupsQuery.data TotalCount', channelGroupsQuery.data.totalItemCount);
    if (!channelGroupsQuery.data?.data) {
      return;
    }

    // if (props.hideControls === true) {
    //   setDataSource(channelGroupsQuery.data.data.filter((item) => item.isHidden !== true));
    // } else {
    setDataSource(channelGroupsQuery.data);
    // }

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

  // const onsetSelectedChannelGroups = React.useCallback((selectedData: StreamMasterApi.ChannelGroupDto | StreamMasterApi.ChannelGroupDto[]) => {

  //   if (Array.isArray(selectedData)) {
  //     const newDatas = selectedData.filter((cg) => cg.id !== undefined);
  //     setSelectedChannelGroups(newDatas);
  //     props.onSelectionChange?.(newDatas);
  //   } else {
  //     setSelectedChannelGroups([selectedData]);
  //     props.onSelectionChange?.([selectedData]);
  //   }

  // }, [props, setSelectedChannelGroups]);


  const sourceColumns = React.useMemo((): ColumnMeta[] => {
    return [
      { field: 'name', filter: true, sortable: true },
      { field: 'regexMatch', filter: true, sortable: true },
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

  const setThin = (filterInfo: DataTableFilterEvent): DataTableFilterMetaData[] => {
    const tosend = [] as DataTableFilterMetaData[];
    if (filterInfo.filters === undefined) {
      return [] as DataTableFilterMetaData[];
    }

    Object.keys(filterInfo.filters).forEach((key) => {
      const value = filterInfo.filters[key] as DataTableFilterMetaData;
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
    setFiltersMeta(tosend);
    return tosend;
  }

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
                // setThin(filtersMeta);
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
          setThin(filterInfo);

        }}

        onPage={(pageInfo) => {
          console.debug(pageInfo.page, pageInfo.first, pageInfo.rows, pageInfo.pageCount, pageInfo.pageCount);
          if (pageInfo.page !== undefined) {
            setPageNumber(pageInfo.page + 1);
          }

          if (pageInfo.rows !== undefined) {
            setPageSize(pageInfo.rows);
          }
        }
        }

        onSelectionChange={(e) => {
          setSelectedChannelGroups(e as StreamMasterApi.ChannelGroupDto[]);
          props.onSelectionChange?.(e as StreamMasterApi.ChannelGroupDto[]);
        }
        }
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
  /**
   * Type of filter match.
   */
  matchMode: 'between' | 'contains' | 'custom' | 'dateAfter' | 'dateBefore' | 'dateIs' | 'dateIsNot' | 'endsWith' | 'equals' | 'gt' | 'gte' | 'in' | 'lt' | 'lte' | 'notContains' | 'notEquals' | 'startsWith' | undefined;
  /**
   * Value to filter against.
   */
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

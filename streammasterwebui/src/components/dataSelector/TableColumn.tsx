
import { type ColumnProps } from "primereact/column";
import { Column } from "primereact/column";
import { camel2title, removeQuotes } from "../../common/common";
import { type ColumnAlign, type ColumnFieldType } from "./DataSelectorTypes";
import { type ColumnMeta } from "./DataSelectorTypes";
import { type Key } from "react";
import { type SyntheticEvent } from "react";
import { type CSSProperties, type ReactNode } from "react";
import { useCallback } from "react";
import { Tooltip } from "primereact/tooltip";
import { v4 as uuidv4 } from 'uuid';
import StreamMasterSetting from "../../store/signlar/StreamMasterSetting";
import { type DataTableValue } from "primereact/datatable";

import getColumnConfig from "./getColumnConfig";
import React from "react";

type TableColumnProp = ColumnProps & {
  column: ColumnMeta;
  groupRowsBy?: string;
  hideControls?: boolean;
};


class TableColumn extends React.Component<TableColumnProp, any> {
  private setting = StreamMasterSetting();

  private readonly tooltipClassName = () => "menuitemds-" + uuidv4();

  private readonly linkIcon = (url: string) => {
    return (
      <a href={url} rel="noopener noreferrer" target="_blank">
        <i className="pi pi-bookmark-fill" />
      </a>
    );
  };

  private readonly linkTemplate = (link: string) => (
    <div>
      <div className="flex justify-content-center align-items-center">
        {this.linkIcon(link)}
      </div>
    </div>
  );

  private readonly getRecord = (data: object, fieldName: string) => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];

    return record;
  };

  private readonly getRecordString = (data: object, fieldName: string): string => {
    type ObjectKey = keyof typeof data;
    const record = data[fieldName as ObjectKey];
    let toDisplay = JSON.stringify(record);

    if (!toDisplay || toDisplay === undefined || toDisplay === '') {
      // console.log("toDisplay is empty for " + fieldName)
      return '';
    }

    toDisplay = removeQuotes(toDisplay);

    return toDisplay;
  };

  private readonly epgSourceTemplate = (tvgid: string) => {
    return (
      <div>
        <div className="flex justify-content-start">
          {tvgid}
        </div>
      </div>
    );
  };

  private readonly imageBodyTemplate = (data: object, fieldName: string) => {
    const record = this.getRecordString(data, fieldName);

    return (
      <div className="flex flex-nowrap justify-content-center align-items-center p-0">
        <img
          alt={record ?? 'Logo'}
          className="max-h-1rem max-w-full p-0"
          onError={(e: SyntheticEvent<HTMLImageElement, Event>) => (e.currentTarget.src = (e.currentTarget.src = this.setting.defaultIcon))}
          src={`${encodeURI(record ?? '')}`}
          style={{
            objectFit: 'contain',
          }}
        />
      </div>
    );
  };

  private readonly streamsBodyTemplate = (activeCount: string, totalCount: string) => {
    if (activeCount === null || totalCount === undefined) {
      return null;
    }

    return (
      <div className="flex align-items-center gap-2" >
        {activeCount}/{totalCount}
      </div>
    );

  };



  private readonly getAlign = (align: ColumnAlign | null | undefined, fieldType: ColumnFieldType): ColumnAlign => {

    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (align === undefined || align === null) {
      return 'left'
    }

    return align;

  };

  private readonly getStyle = (style: CSSProperties | undefined, fieldType: ColumnFieldType | undefined): CSSProperties | undefined => {

    if (fieldType === 'blank') {
      return {
        maxWidth: '1rem',
        width: '1rem',
      } as CSSProperties;
    }

    if (fieldType === 'image' || fieldType === 'm3ulink' || fieldType === 'epglink' || fieldType === 'url') {
      return {
        maxWidth: '5rem',
        width: '5rem',
      } as CSSProperties;
    }

    return {
      ...style,
      flexGrow: 0,
      flexShrink: 1,
      // maxWidth: '10rem',
      overflow: 'hidden',
      textOverflow: 'ellipsis',
      whiteSpace: 'nowrap',

    } as CSSProperties;
  };

  private readonly getAlignHeader = (align: ColumnAlign | undefined, fieldType: ColumnFieldType): ColumnAlign => {
    if (fieldType === 'image') {
      return 'center'
    }

    if (fieldType === 'isHidden') {
      return 'center'
    }

    if (!align) {
      return 'center'
    }

    return align;
  };

  private readonly getFilter = (filter: boolean | undefined, fieldType: ColumnFieldType): boolean | undefined => {
    if (fieldType === 'image') {
      return false;
    }

    return filter;
  }

  private readonly getHeader = (field: string, header: string | undefined, fieldType: ColumnFieldType | undefined): ReactNode => {
    if (!fieldType) {
      return header ? header : camel2title(field);
    }

    switch (fieldType) {
      case 'blank':
        return <div />;
      case 'epg':
        return 'EPG';
      case 'm3ulink':
        return 'M3U';
      case 'epglink':
        return 'XMLTV';
      case 'url':
        return 'HDHR URL';
      case 'streams':
        return (
          <>
            <Tooltip target={"." + this.tooltipClassName} />
            <div
              className={this.tooltipClassName + " border-white"}
              data-pr-at="right+5 top"
              data-pr-hidedelay={100}
              data-pr-my="left center-2"
              data-pr-position="right"
              data-pr-showdelay={200}
              data-pr-tooltip="Active/Total Count"
            >
              Streams<br />(active/total)
            </div>
          </>
        );
      default:
        return header ? header : camel2title(field);
    }
  };


  // public override render() {
  //   const { column, groupRowsBy, hideControls } = this.props;
  //   console.debug("TableColumn.render: column = ", column)
  //   return (
  //     <Column
  //       align={this.getAlign(column.align, column.fieldType)}
  //       alignHeader={this.getAlignHeader(column.align, column.fieldType)}
  //       body={(e) => column.bodyTemplate ? column.bodyTemplate(e) : this.bodyTemplate(e, column.field, column.fieldType, column.camelize)}
  //       editor={column.editor}
  //       field={column.field}
  //       filter={this.getFilter(column.filter, column.fieldType)}
  //       filterElement={column.filterElement}
  //       filterMenuStyle={{ width: '14rem' }}
  //       filterPlaceholder={column.fieldType === 'epg' ? 'EPG' : column.header ? column.header : camel2title(column.field)}
  //       header={this.getHeader(column.field, column.header, column.fieldType)}
  //       hidden={column.isHidden === true || (hideControls === true && this.getHeader(column.field, column.header, column.fieldType) === 'Actions') ? true : undefined}
  //       key={!column.fieldType ? column.field : column.field + column.fieldType}
  //       onCellEditComplete={column.handleOnCellEditComplete}
  //       showAddButton
  //       showApplyButton
  //       showClearButton
  //       showFilterMatchModes
  //       showFilterMenu={column.filterElement === undefined}
  //       showFilterMenuOptions
  //       showFilterOperator
  //       sortable={groupRowsBy === undefined || groupRowsBy === '' ? column.sortable : false}
  //       style={this.getStyle(column.style, column.fieldType)}
  //     />
  //   );
  // }
}

export default TableColumn;

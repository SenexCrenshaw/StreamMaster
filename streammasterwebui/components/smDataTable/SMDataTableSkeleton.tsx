import { camel2title } from '@lib/common/common';
import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';
import { Skeleton } from 'primereact/skeleton';
import { CSSProperties, memo, useMemo } from 'react';

import { getColumnStyles } from './helpers/getColumnStyles';
import { ColumnMeta } from './types/ColumnMeta';

interface SMDataTableSkeletonProps {
  columns: ColumnMeta[];
  itemCount: number;
  style?: CSSProperties;
}

const SMDataTableSkeleton = ({ columns, itemCount = 25, style }: SMDataTableSkeletonProps) => {
  const getFakeData = useMemo(() => {
    const data = [];
    for (let i = 0; i < itemCount; i++) {
      const item = {} as any;
      columns.forEach((column) => {
        const key = column.field as keyof typeof item;
        item[key] = '';
      });
      data.push(item);
    }
    return data;
  }, [columns, itemCount]);

  return (
    <DataTable value={getFakeData} className="p-datatable-striped" style={style}>
      {columns &&
        columns.map((col) => (
          <Column
            key={col.field}
            field={col.field}
            filter
            filterElement={camel2title(col.field)}
            body={<Skeleton />}
            showAddButton
            showApplyButton
            showClearButton={false}
            showFilterMatchModes
            showFilterMenu={false}
            showFilterMenuOptions
            showFilterOperator
            style={getColumnStyles(col)}
          />
        ))}
    </DataTable>
  );
};

export default memo(SMDataTableSkeleton);

import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { useEffect, useState } from 'react';

const useSortedData = <T>(dataKey: string, data: T[] | undefined) => {
  const { sortInfo } = useSelectedAndQ(dataKey);

  const [sortedData, setSortedData] = useState<T[]>([]);

  useEffect(() => {
    if (!data) {
      return;
    }

    if (sortInfo && sortInfo.sortField && sortInfo.sortOrder !== undefined) {
      const sorted = [...data].sort((a, b) => {
        if (a[sortInfo.sortField as keyof T] < b[sortInfo.sortField as keyof T]) {
          return sortInfo.sortOrder === -1 ? 1 : -1;
        }
        if (a[sortInfo.sortField as keyof T] > b[sortInfo.sortField as keyof T]) {
          return sortInfo.sortOrder === -1 ? -1 : 1;
        }
        return 0;
      });
      setSortedData(sorted);
    } else {
      setSortedData(data);
    }
  }, [data, sortInfo]);

  return sortedData;
};

export default useSortedData;

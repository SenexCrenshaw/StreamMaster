'use client';
import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../../../lib/redux/store';

import { StreamGroupDto } from '@/lib/iptvApi';
import { setSelectedStreamGroupInternal } from './selectedStreamGroupSlice';

export const useSelectedStreamGroup = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedStreamGroup = (streamGroup: StreamGroupDto | undefined) => {
    if (streamGroup === undefined) {
      dispatch(
        setSelectedStreamGroupInternal({
          streamGroup: {} as StreamGroupDto,
          typename,
        }),
      );
    } else {
      dispatch(
        setSelectedStreamGroupInternal({
          streamGroup: streamGroup,
          typename,
        }),
      );
    }
  };

  const selectedStreamGroup = useSelector((rootState: RootState) => rootState.selectedStreamGroup[typename]);

  return { selectedStreamGroup, setSelectedStreamGroup };
};

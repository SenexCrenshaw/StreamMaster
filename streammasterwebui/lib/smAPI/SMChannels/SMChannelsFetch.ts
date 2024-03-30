import store from '@lib/redux/store';
import { GetPagedSMChannels } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetPagedSMChannels = createAsyncThunk('cache/getGetPagedSMChannels', async (query: string, thunkAPI) => {
  try {
    if (query === undefined) return;
    const state = store.getState().GetPagedSMChannels;
    // if (state.data[query] !== undefined && !force) return;
    console.log('Fetching GetPagedSMChannels', state.isForced);
    const params = JSON.parse(query);
    const response = await GetPagedSMChannels(params);
    console.log('Fetched GetPagedSMChannels ', response?.data.length);
    return { query: query, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error' });
  }
});

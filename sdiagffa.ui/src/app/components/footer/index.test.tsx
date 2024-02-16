import { describe, expect, it } from 'vitest';
import { render } from '@testing-library/react';
import Footer from '.'

describe('Footer component', () => {
  it('should match snapshot', () => {
    const component = render(<Footer />);
    expect(component).toMatchSnapshot();
  })
});